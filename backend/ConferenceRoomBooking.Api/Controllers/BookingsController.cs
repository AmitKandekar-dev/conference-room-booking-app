using System.ComponentModel.DataAnnotations;
using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.Dtos;
using ConferenceRoomBooking.Api.Models;
using ConferenceRoomBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ConferenceRoomBookingDbContext _dbContext;
    private readonly IEmailService _emailService;

    public BookingsController(ConferenceRoomBookingDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings(CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.BookingSchedules
            .Include(booking => booking.ConferenceRoom)
            .OrderByDescending(booking => booking.BookingDate)
            .ThenBy(booking => booking.StartTime)
            .Select(booking => ToResponse(booking))
            .ToListAsync(cancellationToken);

        return Ok(bookings);
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking([FromBody] BookingRequest request, CancellationToken cancellationToken)
    {
        var validation = await ValidateRequestAsync(request, null, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        var now = DateTime.UtcNow;
        var booking = new BookingSchedule
        {
            ConferenceRoomId = request.ConferenceRoomId!.Value,
            BookingDate = request.BookingDate!.Value,
            StartTime = request.StartTime!.Value,
            EndTime = request.EndTime!.Value,
            MeetingTitle = request.MeetingTitle.Trim(),
            BookedBy = request.BookedBy.Trim(),
            BookedByEmail = request.BookedByEmail.Trim(),
            MachineNameOrWindowsUsername = GetIdentityName(),
            NumberOfPersons = request.NumberOfPersons,
            CreditsUsed = CalculateCredits(request.StartTime.Value, request.EndTime.Value, request.NumberOfPersons),
            CreatedDate = now,
            UpdatedDate = now,
            IsCancelled = false
        };

        _dbContext.BookingSchedules.Add(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await LoadRoomAsync(booking, cancellationToken);
        await SendNotificationAsync(booking, "Created", cancellationToken);

        return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, ToResponse(booking));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(int id, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.BookingSchedules
            .Include(item => item.ConferenceRoom)
            .FirstOrDefaultAsync(item => item.BookingId == id, cancellationToken);

        return booking is null ? NotFound() : Ok(ToResponse(booking));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<BookingResponse>> UpdateBooking(int id, [FromBody] BookingRequest request, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.BookingSchedules
            .Include(item => item.ConferenceRoom)
            .FirstOrDefaultAsync(item => item.BookingId == id, cancellationToken);

        if (booking is null)
        {
            return NotFound();
        }

        if (booking.IsCancelled)
        {
            return BadRequest(new { message = "Cancelled bookings cannot be edited." });
        }

        var validation = await ValidateRequestAsync(request, id, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        booking.ConferenceRoomId = request.ConferenceRoomId!.Value;
        booking.BookingDate = request.BookingDate!.Value;
        booking.StartTime = request.StartTime!.Value;
        booking.EndTime = request.EndTime!.Value;
        booking.MeetingTitle = request.MeetingTitle.Trim();
        booking.BookedBy = request.BookedBy.Trim();
        booking.BookedByEmail = request.BookedByEmail.Trim();
        booking.MachineNameOrWindowsUsername = GetIdentityName();
        booking.NumberOfPersons = request.NumberOfPersons;
        booking.CreditsUsed = CalculateCredits(request.StartTime.Value, request.EndTime.Value, request.NumberOfPersons);
        booking.UpdatedDate = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await LoadRoomAsync(booking, cancellationToken);
        await SendNotificationAsync(booking, "Updated", cancellationToken);

        return Ok(ToResponse(booking));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelBooking(int id, CancellationToken cancellationToken)
    {
        var booking = await _dbContext.BookingSchedules
            .Include(item => item.ConferenceRoom)
            .FirstOrDefaultAsync(item => item.BookingId == id, cancellationToken);

        if (booking is null)
        {
            return NotFound();
        }

        if (!booking.IsCancelled)
        {
            booking.IsCancelled = true;
            booking.CancelledDate = DateTime.UtcNow;
            booking.UpdatedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            await SendNotificationAsync(booking, "Cancelled", cancellationToken);
        }

        return NoContent();
    }

    private async Task<ActionResult?> ValidateRequestAsync(BookingRequest request, int? bookingIdToExclude, CancellationToken cancellationToken)
    {
        var emailValidator = new EmailAddressAttribute();
        if (string.IsNullOrWhiteSpace(request.BookedByEmail) || !emailValidator.IsValid(request.BookedByEmail))
        {
            return BadRequest(new { message = "BookedByEmail is required and must be a valid email address." });
        }

        if (request.StartTime >= request.EndTime)
        {
            return BadRequest(new { message = "End time must be after start time." });
        }

        var roomExists = await _dbContext.ConferenceRooms.AnyAsync(room => room.ConferenceRoomId == request.ConferenceRoomId, cancellationToken);
        if (!roomExists)
        {
            return BadRequest(new { message = "Conference room does not exist." });
        }

        var hasOverlap = await _dbContext.BookingSchedules.AnyAsync(booking =>
            booking.ConferenceRoomId == request.ConferenceRoomId &&
            booking.BookingDate == request.BookingDate &&
            !booking.IsCancelled &&
            booking.BookingId != bookingIdToExclude &&
            request.StartTime < booking.EndTime && booking.StartTime < request.EndTime,
            cancellationToken);

        if (hasOverlap)
        {
            return Conflict(new { message = "The selected conference room is already booked for this time." });
        }

        return null;
    }

    private async Task LoadRoomAsync(BookingSchedule booking, CancellationToken cancellationToken)
    {
        booking.ConferenceRoom = await _dbContext.ConferenceRooms.FindAsync(new object[] { booking.ConferenceRoomId }, cancellationToken);
    }

    private async Task SendNotificationAsync(BookingSchedule booking, string status, CancellationToken cancellationToken)
    {
        await _emailService.SendBookingNotificationAsync(new BookingNotification(
            booking.BookedByEmail,
            booking.BookedBy,
            booking.MeetingTitle,
            booking.ConferenceRoom?.Name ?? string.Empty,
            booking.BookingDate,
            booking.StartTime,
            booking.EndTime,
            booking.NumberOfPersons,
            booking.CreditsUsed,
            status), cancellationToken);
    }

    private string GetIdentityName()
    {
        return User.Identity?.Name ?? Environment.UserName ?? Environment.MachineName;
    }

    private static decimal CalculateCredits(TimeOnly startTime, TimeOnly endTime, int numberOfPersons)
    {
        var durationHours = (decimal)(endTime.ToTimeSpan() - startTime.ToTimeSpan()).TotalHours;
        return Math.Round(durationHours * Math.Max(numberOfPersons, 1), 2, MidpointRounding.AwayFromZero);
    }

    private static BookingResponse ToResponse(BookingSchedule booking)
    {
        return new BookingResponse(
            booking.BookingId,
            booking.ConferenceRoomId,
            booking.ConferenceRoom?.Name ?? string.Empty,
            booking.BookingDate,
            booking.StartTime,
            booking.EndTime,
            booking.MeetingTitle,
            booking.BookedBy,
            booking.BookedByEmail,
            booking.MachineNameOrWindowsUsername,
            booking.NumberOfPersons,
            booking.CreditsUsed,
            booking.CreatedDate,
            booking.UpdatedDate,
            booking.CancelledDate,
            booking.IsCancelled,
            booking.IsCancelled ? "Cancelled" : "Active");
    }
}
