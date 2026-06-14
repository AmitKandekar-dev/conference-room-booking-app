namespace ConferenceRoomBooking.Api.Services;

public record BookingNotification(
    string ToEmail,
    string ToName,
    string MeetingTitle,
    string ConferenceRoomName,
    DateOnly BookingDate,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int NumberOfPersons,
    decimal CreditsUsed,
    string BookingStatus);

public interface IEmailService
{
    Task SendBookingNotificationAsync(BookingNotification notification, CancellationToken cancellationToken = default);
}
