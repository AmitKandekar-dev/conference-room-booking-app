using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ConferenceRoomBookingDbContext _dbContext;

    public AdminController(ConferenceRoomBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAdminBookings(CancellationToken cancellationToken)
    {
        var bookings = await _dbContext.BookingSchedules
            .Include(booking => booking.ConferenceRoom)
            .OrderByDescending(booking => booking.BookingDate)
            .ThenBy(booking => booking.StartTime)
            .Select(booking => new BookingResponse(
                booking.BookingId,
                booking.ConferenceRoomId,
                booking.ConferenceRoom != null ? booking.ConferenceRoom.Name : string.Empty,
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
                booking.IsCancelled ? "Cancelled" : "Active"))
            .ToListAsync(cancellationToken);

        return Ok(bookings);
    }
}
