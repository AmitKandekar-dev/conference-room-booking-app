using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.Dtos;

public class BookingRequest
{
    [Required]
    public int? ConferenceRoomId { get; set; }

    [Required]
    public DateOnly? BookingDate { get; set; }

    [Required]
    public TimeOnly? StartTime { get; set; }

    [Required]
    public TimeOnly? EndTime { get; set; }

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

    [Range(1, 1000)]
    public int NumberOfPersons { get; set; }
}

public record BookingResponse(
    int BookingId,
    int ConferenceRoomId,
    string ConferenceRoomName,
    DateOnly BookingDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string MeetingTitle,
    string BookedBy,
    string BookedByEmail,
    string MachineNameOrWindowsUsername,
    int NumberOfPersons,
    decimal CreditsUsed,
    DateTime CreatedDate,
    DateTime UpdatedDate,
    DateTime? CancelledDate,
    bool IsCancelled,
    string BookingStatus);

public record ConferenceRoomResponse(int ConferenceRoomId, string Name, int Capacity);
