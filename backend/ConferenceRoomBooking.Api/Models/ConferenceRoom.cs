using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.Models;

public class ConferenceRoom
{
    public int ConferenceRoomId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public ICollection<BookingSchedule> Bookings { get; set; } = new List<BookingSchedule>();
}
