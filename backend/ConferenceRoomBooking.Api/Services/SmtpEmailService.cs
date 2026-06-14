using System.Net;
using System.Net.Mail;
using ConferenceRoomBooking.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConferenceRoomBooking.Api.Services;

public class SmtpEmailService : IEmailService
{
    private const string MissingSettingsWarning = "Email notification skipped because SMTP settings are not configured.";
    private readonly ConferenceRoomBookingDbContext _dbContext;
    private readonly EmailSettings _configuredSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        ConferenceRoomBookingDbContext dbContext,
        IOptions<EmailSettings> configuredSettings,
        ILogger<SmtpEmailService> logger)
    {
        _dbContext = dbContext;
        _configuredSettings = configuredSettings.Value;
        _logger = logger;
    }

    public async Task SendBookingNotificationAsync(BookingNotification notification, CancellationToken cancellationToken = default)
    {
        var settings = await GetSettingsAsync(cancellationToken);
        if (!IsConfigured(settings))
        {
            _logger.LogWarning(MissingSettingsWarning);
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(settings.FromEmail, settings.FromName),
                Subject = $"Conference room booking {notification.BookingStatus}: {notification.MeetingTitle}",
                Body = BuildBody(notification),
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress(notification.ToEmail, notification.ToName));

            using var smtpClient = new SmtpClient(settings.SmtpHost, settings.SmtpPort)
            {
                EnableSsl = settings.EnableSsl
            };

            if (!string.IsNullOrWhiteSpace(settings.Username))
            {
                smtpClient.Credentials = new NetworkCredential(settings.Username, settings.Password);
            }

            await smtpClient.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Email notification could not be sent for booking status {BookingStatus} to {BookedByEmail}.", notification.BookingStatus, notification.ToEmail);
        }
    }

    private async Task<EmailSettings> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var databaseSettings = await _dbContext.EmailSettings
            .AsNoTracking()
            .Where(settings => settings.IsActive)
            .OrderBy(settings => settings.EmailSettingsId)
            .FirstOrDefaultAsync(cancellationToken);

        return databaseSettings ?? _configuredSettings;
    }

    private static bool IsConfigured(EmailSettings settings)
    {
        return !string.IsNullOrWhiteSpace(settings.SmtpHost)
            && settings.SmtpPort > 0
            && !string.IsNullOrWhiteSpace(settings.FromEmail)
            && new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(settings.FromEmail);
    }

    private static string BuildBody(BookingNotification notification)
    {
        return string.Join(Environment.NewLine, new[]
        {
            $"Booking status: {notification.BookingStatus}",
            $"Meeting title: {notification.MeetingTitle}",
            $"Conference room name: {notification.ConferenceRoomName}",
            $"Booking date: {notification.BookingDate:yyyy-MM-dd}",
            $"Start time: {notification.StartTime:HH:mm}",
            $"End time: {notification.EndTime:HH:mm}",
            $"Number of persons: {notification.NumberOfPersons}",
            $"Credits used: {notification.CreditsUsed:0.##}"
        });
    }
}
