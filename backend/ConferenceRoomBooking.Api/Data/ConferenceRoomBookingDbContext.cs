using ConferenceRoomBooking.Api.Models;
using ConferenceRoomBooking.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ConferenceRoomBooking.Api.Data;

public class ConferenceRoomBookingDbContext : DbContext
{
    public ConferenceRoomBookingDbContext(DbContextOptions<ConferenceRoomBookingDbContext> options) : base(options)
    {
    }

    public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();
    public DbSet<BookingSchedule> BookingSchedules => Set<BookingSchedule>();
    public DbSet<EmailSettings> EmailSettings => Set<EmailSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConferenceRoom>(entity =>
        {
            entity.HasKey(room => room.ConferenceRoomId);
            entity.Property(room => room.Name).IsRequired().HasMaxLength(150);
            entity.HasData(
                new ConferenceRoom { ConferenceRoomId = 1, Name = "Board Room", Capacity = 12 },
                new ConferenceRoom { ConferenceRoomId = 2, Name = "Training Room", Capacity = 30 },
                new ConferenceRoom { ConferenceRoomId = 3, Name = "Focus Room", Capacity = 6 });
        });

        modelBuilder.Entity<BookingSchedule>(entity =>
        {
            entity.HasKey(booking => booking.BookingId);
            entity.Property(booking => booking.MeetingTitle).IsRequired().HasMaxLength(200);
            entity.Property(booking => booking.BookedBy).IsRequired().HasMaxLength(150);
            entity.Property(booking => booking.BookedByEmail).IsRequired().HasMaxLength(254);
            entity.Property(booking => booking.MachineNameOrWindowsUsername).HasMaxLength(256);
            entity.Property(booking => booking.CreditsUsed).HasColumnType("decimal(10,2)");
            entity.Property(booking => booking.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(booking => booking.UpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasIndex(booking => new { booking.ConferenceRoomId, booking.BookingDate, booking.StartTime, booking.EndTime });
            entity.HasOne(booking => booking.ConferenceRoom)
                .WithMany(room => room.Bookings)
                .HasForeignKey(booking => booking.ConferenceRoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EmailSettings>(entity =>
        {
            entity.HasKey(settings => settings.EmailSettingsId);
            entity.Property(settings => settings.SmtpHost).HasMaxLength(200);
            entity.Property(settings => settings.FromEmail).HasMaxLength(254);
            entity.Property(settings => settings.FromName).HasMaxLength(150);
            entity.Property(settings => settings.Username).HasMaxLength(200);
            entity.Property(settings => settings.Password).HasMaxLength(500);
            entity.Property(settings => settings.UpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasData(new EmailSettings
            {
                EmailSettingsId = 1,
                SmtpHost = "smtp.company.com",
                SmtpPort = 587,
                EnableSsl = true,
                FromEmail = "conference-booking@company.com",
                FromName = "Conference Room Booking",
                Username = "",
                Password = "",
                IsActive = true,
                UpdatedDate = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc)
            });
        });
    }
}
