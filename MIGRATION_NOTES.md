# Migration Notes

## What Was Implemented

- Created a new ASP.NET Core API targeting `net10.0`.
- Created a new Angular frontend targeting Angular 22.
- Added SQL Server entities for `ConferenceRooms`, `BookingSchedules`, and database-backed `EmailSettings`.
- Added `BookedByEmail` capture on the booking form.
- Added frontend validation for required and valid email addresses.
- Added backend validation for `BookedByEmail` before saving bookings.
- Added backend-only notification triggers after successful create, update, and cancel operations.
- Added admin dashboard visibility for booked by, booked by email, room, date/time, number of persons, credits used, and booking status.

## Version Decisions

- .NET 10 LTS was selected for the backend because it is the latest production LTS line.
- Angular 22 was selected for the frontend because it is actively supported and compatible with Node.js 24 LTS.
- Node.js 24 LTS and npm 11.x are required for clean frontend installs and builds.

## Breaking Changes Addressed

- The API uses the latest minimal hosting model in `Program.cs`.
- EF Core packages are aligned to the target .NET version.
- The frontend uses standalone Angular bootstrapping and the Angular 22 application builder.
- Date and time values are modeled explicitly with `DateOnly` and `TimeOnly` in the backend.
- SMTP email delivery is isolated from booking persistence so a mail failure does not roll back a successful booking.

## Manual Steps Required

1. Configure `ConnectionStrings:DefaultConnection` for the target SQL Server instance.
2. Run `dotnet tool restore` if `dotnet-ef` is not globally installed.
3. Run `dotnet ef database update` or `dotnet tool run dotnet-ef database update`.
4. Update the `EmailSettings` SQL table with real SMTP credentials.
5. For IIS hosting, install the .NET 10 ASP.NET Core Hosting Bundle.

## Validation Completed

- Backend `dotnet restore` completed successfully.
- Backend `dotnet build --no-restore` completed successfully with 0 warnings and 0 errors.
- EF Core InitialCreate migration generated successfully.
- EF Core database update completed successfully against LocalDB and created the ConferenceRoomBooking database.
- Frontend `npm install` completed successfully and generated `package-lock.json`.
- Frontend production 
pm run build completed successfully.
- Runtime smoke checks returned HTTP 200 for http://localhost:4200 and https://localhost:7185/api/conferencerooms.
- `npm audit --omit=dev` reports 0 production vulnerabilities.

## Known Limitations

- This folder did not contain an existing application when implementation started, so the project was created from scratch instead of modifying prior source files.
- Full dev dependency audit reports 3 high severity issues in Angular build tooling dependencies (`@angular/build` -> Vite/esbuild). npm reports no fix currently available for those dev-only advisories.