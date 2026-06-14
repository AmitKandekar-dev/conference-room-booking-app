using System.ComponentModel.DataAnnotations;

namespace ConferenceRoomBooking.Api.Services;

public class EmailSettings
{
    public int EmailSettingsId { get; set; }

    [MaxLength(200)]
    public string SmtpHost { get; set; } = string.Empty;

    public int SmtpPort { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;

    [MaxLength(254)]
    public string FromEmail { get; set; } = string.Empty;

    [MaxLength(150)]
    public string FromName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Password { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
