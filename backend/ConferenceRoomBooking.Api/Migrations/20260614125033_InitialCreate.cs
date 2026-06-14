using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConferenceRoomBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConferenceRooms",
                columns: table => new
                {
                    ConferenceRoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceRooms", x => x.ConferenceRoomId);
                });

            migrationBuilder.CreateTable(
                name: "EmailSettings",
                columns: table => new
                {
                    EmailSettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmtpHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SmtpPort = table.Column<int>(type: "int", nullable: false),
                    EnableSsl = table.Column<bool>(type: "bit", nullable: false),
                    FromEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSettings", x => x.EmailSettingsId);
                });

            migrationBuilder.CreateTable(
                name: "BookingSchedules",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConferenceRoomId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    MeetingTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BookedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    BookedByEmail = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    MachineNameOrWindowsUsername = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NumberOfPersons = table.Column<int>(type: "int", nullable: false),
                    CreditsUsed = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSchedules", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_BookingSchedules_ConferenceRooms_ConferenceRoomId",
                        column: x => x.ConferenceRoomId,
                        principalTable: "ConferenceRooms",
                        principalColumn: "ConferenceRoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ConferenceRooms",
                columns: new[] { "ConferenceRoomId", "Capacity", "Name" },
                values: new object[,]
                {
                    { 1, 12, "Board Room" },
                    { 2, 30, "Training Room" },
                    { 3, 6, "Focus Room" }
                });

            migrationBuilder.InsertData(
                table: "EmailSettings",
                columns: new[] { "EmailSettingsId", "EnableSsl", "FromEmail", "FromName", "IsActive", "Password", "SmtpHost", "SmtpPort", "UpdatedDate", "Username" },
                values: new object[] { 1, true, "conference-booking@company.com", "Conference Room Booking", true, "", "smtp.company.com", 587, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSchedules_ConferenceRoomId_BookingDate_StartTime_EndTime",
                table: "BookingSchedules",
                columns: new[] { "ConferenceRoomId", "BookingDate", "StartTime", "EndTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSchedules");

            migrationBuilder.DropTable(
                name: "EmailSettings");

            migrationBuilder.DropTable(
                name: "ConferenceRooms");
        }
    }
}
