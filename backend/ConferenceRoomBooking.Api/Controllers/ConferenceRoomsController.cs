using ConferenceRoomBooking.Api.Data;
using ConferenceRoomBooking.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConferenceRoomsController : ControllerBase
{
    private readonly ConferenceRoomBookingDbContext _dbContext;

    public ConferenceRoomsController(ConferenceRoomBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConferenceRoomResponse>>> GetRooms(CancellationToken cancellationToken)
    {
        var rooms = await _dbContext.ConferenceRooms
            .OrderBy(room => room.Name)
            .Select(room => new ConferenceRoomResponse(room.ConferenceRoomId, room.Name, room.Capacity))
            .ToListAsync(cancellationToken);

        return Ok(rooms);
    }
}
