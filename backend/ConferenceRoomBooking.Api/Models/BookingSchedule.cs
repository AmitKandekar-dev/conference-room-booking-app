using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.Models;

public class BookingSchedule
{
    public int BookingId { get; set; }
    public int ConferenceRoomId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    [Required]
    [MaxLength(200)]
    public string MeetingTitle { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string BookedBy { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string BookedByEmail { get; set; } = string.Empty;

    [MaxLength(256)]
    public string MachineNameOrWindowsUsername { get; set; } = string.Empty;

    public int NumberOfPersons { get; set; }
    public decimal CreditsUsed { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public bool IsCancelled { get; set; }

    public ConferenceRoom? ConferenceRoom { get; set; }
}
