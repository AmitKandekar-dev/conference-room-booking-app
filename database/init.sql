IF DB_ID(N'ConferenceRoomBooking') IS NULL
BEGIN
    CREATE DATABASE ConferenceRoomBooking;
END
GO

USE ConferenceRoomBooking;
GO

IF OBJECT_ID(N'dbo.ConferenceRooms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ConferenceRooms
    (
        ConferenceRoomId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ConferenceRooms PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Capacity INT NOT NULL
    );

    INSERT INTO dbo.ConferenceRooms (Name, Capacity)
    VALUES (N'Board Room', 12), (N'Training Room', 30), (N'Focus Room', 6);
END
GO

IF OBJECT_ID(N'dbo.EmailSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmailSettings
    (
        EmailSettingsId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmailSettings PRIMARY KEY,
        SmtpHost NVARCHAR(200) NOT NULL,
        SmtpPort INT NOT NULL CONSTRAINT DF_EmailSettings_SmtpPort DEFAULT 587,
        EnableSsl BIT NOT NULL CONSTRAINT DF_EmailSettings_EnableSsl DEFAULT 1,
        FromEmail NVARCHAR(254) NOT NULL,
        FromName NVARCHAR(150) NOT NULL,
        Username NVARCHAR(200) NOT NULL CONSTRAINT DF_EmailSettings_Username DEFAULT N'',
        Password NVARCHAR(500) NOT NULL CONSTRAINT DF_EmailSettings_Password DEFAULT N'',
        IsActive BIT NOT NULL CONSTRAINT DF_EmailSettings_IsActive DEFAULT 1,
        UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_EmailSettings_UpdatedDate DEFAULT SYSUTCDATETIME()
    );

    INSERT INTO dbo.EmailSettings (SmtpHost, SmtpPort, EnableSsl, FromEmail, FromName, Username, Password, IsActive)
    VALUES (N'smtp.company.com', 587, 1, N'conference-booking@company.com', N'Conference Room Booking', N'', N'', 1);
END
GO

IF OBJECT_ID(N'dbo.BookingSchedules', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingSchedules
    (
        BookingId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BookingSchedules PRIMARY KEY,
        ConferenceRoomId INT NOT NULL,
        BookingDate DATE NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        MeetingTitle NVARCHAR(200) NOT NULL,
        BookedBy NVARCHAR(150) NOT NULL,
        BookedByEmail NVARCHAR(254) NOT NULL,
        MachineNameOrWindowsUsername NVARCHAR(256) NOT NULL,
        NumberOfPersons INT NOT NULL,
        CreditsUsed DECIMAL(10,2) NOT NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_BookingSchedules_CreatedDate DEFAULT SYSUTCDATETIME(),
        UpdatedDate DATETIME2 NOT NULL CONSTRAINT DF_BookingSchedules_UpdatedDate DEFAULT SYSUTCDATETIME(),
        CancelledDate DATETIME2 NULL,
        IsCancelled BIT NOT NULL CONSTRAINT DF_BookingSchedules_IsCancelled DEFAULT 0,
        CONSTRAINT FK_BookingSchedules_ConferenceRooms FOREIGN KEY (ConferenceRoomId) REFERENCES dbo.ConferenceRooms(ConferenceRoomId)
    );

    CREATE INDEX IX_BookingSchedules_Room_Date_Time
        ON dbo.BookingSchedules (ConferenceRoomId, BookingDate, StartTime, EndTime);
END
GO
