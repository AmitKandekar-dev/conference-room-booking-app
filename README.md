# Conference Room Booking App

Angular + ASP.NET Core + SQL Server conference room booking application with backend email notifications for booking creation, edits, and cancellations.

## Required Runtime Versions

- .NET SDK: 10.0.x LTS, tested with 10.0.301
- Node.js: 24.x LTS, tested with 24.16.0
- npm: 11.x, tested with 11.17.0
- SQL Server or SQL Server LocalDB

These versions were selected because .NET 10 is the current Microsoft LTS runtime and Angular 22 supports Node `^24.15.0`.

## Backend Setup

```powershell
cd C:\AmitKandekar\GHSA\conference-room-booking-app\backend\ConferenceRoomBooking.Api
dotnet --version
dotnet restore
dotnet tool restore
dotnet build
dotnet ef database update
dotnet run
```

If `dotnet ef` is not installed globally, use the local tool manifest instead:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update
```

The API runs on:

- `https://localhost:7185`
- `http://localhost:5185`

## Frontend Setup

```powershell
cd C:\AmitKandekar\GHSA\conference-room-booking-app\frontend
node -v
npm -v
npm install
npm run build
npm start
```

The Angular app runs on `http://localhost:4200` and calls the API at `http://localhost:5185/api` during local development. HTTPS remains available at `https://localhost:7185` for trusted-certificate scenarios and non-development hosting.

## Database Setup

Use the included EF Core migration:

```powershell
cd C:\AmitKandekar\GHSA\conference-room-booking-app\backend\ConferenceRoomBooking.Api
dotnet ef database update
```

Create a new migration after model changes:

```powershell
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

Or run the SQL script manually:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i C:\AmitKandekar\GHSA\conference-room-booking-app\database\init.sql
```

## SMTP Settings

SMTP settings are stored in SQL Server in the `EmailSettings` table. `appsettings.json` contains the same shape as a fallback/default seed:

```json
"EmailSettings": {
  "SmtpHost": "smtp.company.com",
  "SmtpPort": 587,
  "EnableSsl": true,
  "FromEmail": "conference-booking@company.com",
  "FromName": "Conference Room Booking",
  "Username": "",
  "Password": ""
}
```

If SMTP settings are missing or incomplete, bookings are still saved. The API logs:

```text
Email notification skipped because SMTP settings are not configured.
```

## Publish for IIS

```powershell
cd C:\AmitKandekar\GHSA\conference-room-booking-app\backend\ConferenceRoomBooking.Api
dotnet publish -c Release -o .\publish
```

Install the ASP.NET Core Hosting Bundle for .NET 10 on the IIS server, configure the IIS site to point at the published folder, and set the production connection string in `appsettings.Production.json` or IIS environment variables.