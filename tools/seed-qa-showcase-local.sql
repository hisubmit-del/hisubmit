/*
    HiSubmit local QA showcase seed
    Target: local HiSubmitDB50 only.

    Accounts (local-only):
      qa.artist@hisubmit.test   / 123Pa$$word!
      qa.festival@hisubmit.test / 123Pa$$word!
      qa.referee@hisubmit.test  / 123Pa$$word!

    The password hash is copied from the existing local johndoe account.
    This script is idempotent by the [QA] festival marker.
*/

USE [HiSubmitDB50];
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRANSACTION;

IF EXISTS
(
    SELECT 1
    FROM [hisubmi1_user].[Festivals]
    WHERE [Name] = N'[QA] Complete Creative Showcase 2026'
)
BEGIN
    PRINT 'QA showcase seed already exists. No changes were made.';
    ROLLBACK TRANSACTION;
    RETURN;
END;

DECLARE @now datetime2 = SYSUTCDATETIME();
DECLARE @passwordHash nvarchar(max) =
(
    SELECT TOP (1) [PasswordHash]
    FROM [Identity].[Users]
    WHERE [UserName] = N'johndoe'
);

IF @passwordHash IS NULL
    THROW 51100, 'The local johndoe account was not found; QA seed aborted.', 1;

DECLARE @artistId nvarchar(450);
DECLARE @festivalUserId nvarchar(450);
DECLARE @refereeId nvarchar(450);

SELECT @artistId = [Id]
FROM [Identity].[Users]
WHERE [UserName] = N'qa.artist';

IF @artistId IS NULL
BEGIN
    SET @artistId = CONVERT(nvarchar(450), NEWID());
    INSERT INTO [Identity].[Users]
    (
        [Id], [FirstName], [LastName], [CreatedOn], [IsDeleted], [IsActive],
        [RefreshTokenExpiryTime], [FeeStatus], [UserName], [NormalizedUserName],
        [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash],
        [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed],
        [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount]
    )
    VALUES
    (
        @artistId, N'QA', N'Artist', @now, 0, 1, '0001-01-01', 0,
        N'qa.artist', N'QA.ARTIST', N'qa.artist@hisubmit.test',
        N'QA.ARTIST@HISUBMIT.TEST', 1, @passwordHash,
        CONVERT(nvarchar(450), NEWID()), CONVERT(nvarchar(450), NEWID()),
        1, 0, 1, 0
    );
END;

SELECT @festivalUserId = [Id]
FROM [Identity].[Users]
WHERE [UserName] = N'qa.festival';

IF @festivalUserId IS NULL
BEGIN
    SET @festivalUserId = CONVERT(nvarchar(450), NEWID());
    INSERT INTO [Identity].[Users]
    (
        [Id], [FirstName], [LastName], [CreatedOn], [IsDeleted], [IsActive],
        [RefreshTokenExpiryTime], [FeeStatus], [UserName], [NormalizedUserName],
        [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash],
        [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed],
        [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount]
    )
    VALUES
    (
        @festivalUserId, N'QA', N'Festival Manager', @now, 0, 1, '0001-01-01', 0,
        N'qa.festival', N'QA.FESTIVAL', N'qa.festival@hisubmit.test',
        N'QA.FESTIVAL@HISUBMIT.TEST', 1, @passwordHash,
        CONVERT(nvarchar(450), NEWID()), CONVERT(nvarchar(450), NEWID()),
        1, 0, 1, 0
    );
END;

SELECT @refereeId = [Id]
FROM [Identity].[Users]
WHERE [UserName] = N'qa.referee';

IF @refereeId IS NULL
BEGIN
    SET @refereeId = CONVERT(nvarchar(450), NEWID());
    INSERT INTO [Identity].[Users]
    (
        [Id], [FirstName], [LastName], [CreatedOn], [IsDeleted], [IsActive],
        [RefreshTokenExpiryTime], [FeeStatus], [UserName], [NormalizedUserName],
        [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash],
        [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed],
        [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount]
    )
    VALUES
    (
        @refereeId, N'QA', N'Referee', @now, 0, 1, '0001-01-01', 0,
        N'qa.referee', N'QA.REFEREE', N'qa.referee@hisubmit.test',
        N'QA.REFEREE@HISUBMIT.TEST', 1, @passwordHash,
        CONVERT(nvarchar(450), NEWID()), CONVERT(nvarchar(450), NEWID()),
        1, 0, 1, 0
    );
END;

DECLARE @artistRoleId nvarchar(450) =
(
    SELECT TOP (1) [Id]
    FROM [Identity].[Roles]
    WHERE [Name] = N'Artist' AND [FestivalId] IS NULL
);
DECLARE @festivalRoleId nvarchar(450) =
(
    SELECT TOP (1) [Id]
    FROM [Identity].[Roles]
    WHERE [Name] = N'Festival' AND [FestivalId] IS NULL
);

IF @artistRoleId IS NULL OR @festivalRoleId IS NULL
    THROW 51101, 'Global Artist/Festival roles were not found; QA seed aborted.', 1;

IF NOT EXISTS (SELECT 1 FROM [Identity].[UserRoles] WHERE [UserId] = @artistId AND [RoleId] = @artistRoleId)
    INSERT INTO [Identity].[UserRoles] ([UserId], [RoleId]) VALUES (@artistId, @artistRoleId);

IF NOT EXISTS (SELECT 1 FROM [Identity].[UserRoles] WHERE [UserId] = @festivalUserId AND [RoleId] = @festivalRoleId)
    INSERT INTO [Identity].[UserRoles] ([UserId], [RoleId]) VALUES (@festivalUserId, @festivalRoleId);

DECLARE @masterId int;
INSERT INTO [hisubmi1_user].[FestivalMasters]
(
    [Name], [ActivePeriod], [ActiveId], [UserId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Complete Creative Showcase Master', 2026, 0,
    @festivalUserId, @festivalUserId, @now
);
SET @masterId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @festivalId int;
INSERT INTO [hisubmi1_user].[Festivals]
(
    [FestivalMasterId], [Name], [UserId], [Description], [LogoURL], [Rules],
    [YearsRunning], [RewardsName], [RewardLogoURL], [Rewards],
    [AudienceAttendence], [EstimatedSubmissions], [ProjectsSelected],
    [AwardsPresented], [EventType], [FilmFestival], [ScreenWritingWriter],
    [MusicContest], [PhotographicContest], [OnlineFestival], [ArtFestival],
    [WebSite], [Email], [Phone], [Facebook], [Twitter], [Instagram],
    [WhatsAppNumber], [Telegram], [Youtube], [SeparateSubmissiionAddress],
    [OnlineEvent], [OpeningDate], [NotificationDate], [EventStartDate],
    [EventEndDate], [Public], [SearchTerms], [AllLenghtAccepted],
    [MinimomLenght], [MaximomLenght], [URL], [StartingNumber], [Prefix],
    [ApprovedLicenseURL], [IsActive], [FestivalStatus], [FeeStatus],
    [IsActivePeriod], [MinFee], [MaxFee], [CreatedBy], [CreatedOn]
)
VALUES
(
    @masterId, N'[QA] Complete Creative Showcase 2026', @festivalUserId,
    N'A complete local showcase for validating every project type, festival form, deadlines, submissions, judging access, media previews and public festival pages.',
    N'/img/ArtWall.jpg',
    N'Original work only. English metadata is required. The artist retains copyright. By submitting, the artist grants the festival a non-exclusive right to screen or display the work for judging and festival promotion.',
    1, N'QA Showcase Awards', N'/img/FestivalQualifying/academy.png',
    N'Best in Show; Emerging Artist; Jury Mention. Each selected project receives a digital certificate and public recognition.',
    500, 250, 60, 12, 6, 1, 1, 1, 1, 1, 1,
    N'https://example.test/qa-complete-creative-showcase-2026',
    N'qa.festival@hisubmit.test', N'+1 416 555 2026',
    N'https://facebook.com/', N'https://twitter.com/', N'https://instagram.com/',
    N'+1 416 555 2026', N'https://t.me/', N'https://youtube.com/',
    1, 0, '2026-08-01', '2026-10-10', '2026-10-24', '2026-10-31',
    1, N'QA showcase film photography music script VR XR art Canada international',
    0, 1, 180, N'qa-complete-creative-showcase-2026', 2600, N'QA26',
    N'/media/qa/qa-script.pdf', 1, 2, 0, 1, 15, 75,
    @festivalUserId, @now
);
SET @festivalId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @refereeRoleId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
INSERT INTO [Identity].[Roles]
(
    [Id], [Description], [CreatedBy], [CreatedOn], [Name], [NormalizedName],
    [ConcurrencyStamp], [FestivalId]
)
VALUES
(
    @refereeRoleId, N'QA referee role scoped only to the Complete Creative Showcase 2026 festival.',
    @festivalUserId, @now, N'Referee', N'REFEREE', CONVERT(nvarchar(450), NEWID()),
    @festivalId
);

IF NOT EXISTS (SELECT 1 FROM [Identity].[UserRoles] WHERE [UserId] = @refereeId AND [RoleId] = @refereeRoleId)
    INSERT INTO [Identity].[UserRoles] ([UserId], [RoleId]) VALUES (@refereeId, @refereeRoleId);

INSERT INTO [hisubmi1_user].[FestivalSubUser]
(
    [FestivalId], [UserId], [IsReferee], [IsRemoved], [CreatedBy], [CreatedOn]
)
VALUES
    (@festivalId, @festivalUserId, 0, 0, @festivalUserId, @now),
    (@festivalId, @refereeId, 1, 0, @festivalUserId, @now);

DECLARE @addressId int;
INSERT INTO [hisubmi1_user].[Addresses]
(
    [Text], [City], [State], [PostalCode], [CountryId], [FestivalId],
    [MapLocation], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Creative Arts Centre, 100 Harbour Street', N'Toronto', N'Ontario',
    N'M5J 2N1', 44, @festivalId, N'43.6426,-79.3871', @festivalUserId, @now
);
SET @addressId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @submissionAddressId int;
INSERT INTO [hisubmi1_user].[Addresses]
(
    [Text], [City], [State], [PostalCode], [CountryId], [SubmissionFestivalId],
    [MapLocation], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'QA Showcase Submissions, PO Box 2026', N'Toronto', N'Ontario',
    N'M5V 3A8', 44, @festivalId, N'43.6532,-79.3832', @festivalUserId, @now
);
SET @submissionAddressId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @venueId int;
INSERT INTO [hisubmi1_user].[Venues]
(
    [Name], [FestivalId], [AddressId], [VenueType], [Capacity],
    [AvailableCapacity], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'QA Showcase Main Gallery', @festivalId, @addressId, 2, 500, 500,
    @festivalUserId, @now
);
SET @venueId = CONVERT(int, SCOPE_IDENTITY());

UPDATE [hisubmi1_user].[Addresses] SET [VenueId] = @venueId WHERE [Id] = @addressId;

INSERT INTO [hisubmi1_user].[EventOrginizers]
(
    [Name], [Title], [FestivalId], [ImageName], [CreatedBy], [CreatedOn]
)
VALUES
    (N'Alex Morgan', N'Artistic Director', @festivalId, N'/assets/img/avatar.png', @festivalUserId, @now),
    (N'Riley Chen', N'Production Manager', @festivalId, N'/assets/img/avatar.png', @festivalUserId, @now);

INSERT INTO [hisubmi1_user].[Images]
(
    [Title], [Url], [ImageType], [FestivalId], [CreatedBy], [CreatedOn]
)
VALUES
    (N'QA Showcase hero image', N'/img/ArtWall.jpg', 0, @festivalId, @festivalUserId, @now),
    (N'QA Showcase gallery image', N'/img/1.jpg', 0, @festivalId, @festivalUserId, @now);

INSERT INTO [hisubmi1_user].[FestivalFiles]
(
    [Name], [FileURL], [FileFormat], [Description], [FestivalId],
    [CreatedBy], [CreatedOn]
)
VALUES
    (N'QA Showcase rules PDF', N'/media/qa/qa-script.pdf', 6,
     N'Complete rules and eligibility document for local QA testing.',
     @festivalId, @festivalUserId, @now);

DECLARE @catFilm int;
DECLARE @catPhoto int;
DECLARE @catMusic int;
DECLARE @catScript int;
DECLARE @catXr int;
DECLARE @catArt int;

INSERT INTO [hisubmi1_user].[EventCategories]
(
    [Name], [Description], [FestivalId], [ProjectType], [RuntimeType],
    [FirstRunTimeValue], [SecoundRunTimeValue], [RequirePassword],
    [Password], [StudentProject], [LocationType], [CityOrStateName],
    [CreatedBy], [CreatedOn]
)
VALUES
    (N'QA Narrative Film', N'Films up to 180 minutes.', @festivalId, 1, 3, 180, NULL, 0, NULL, 0, 3, N'Toronto', @festivalUserId, @now),
    (N'QA Photography Portfolio', N'Photography series and single images.', @festivalId, 2, NULL, 0, NULL, 0, NULL, 0, 1, N'Canada', @festivalUserId, @now),
    (N'QA Original Music', N'Original songs, scores and sound works.', @festivalId, 3, 3, 30, NULL, 0, NULL, 0, 1, N'Canada', @festivalUserId, @now),
    (N'QA Screenwriting', N'Screenplays and short scripts.', @festivalId, 4, NULL, 120, NULL, 0, NULL, 0, 1, N'Canada', @festivalUserId, @now),
    (N'QA Immersive XR', N'VR/XR/360 immersive experiences.', @festivalId, 5, 3, 60, NULL, 0, NULL, 0, 3, N'Toronto', @festivalUserId, @now),
    (N'QA Visual Art', N'Visual art, installation and mixed media.', @festivalId, 6, NULL, 0, NULL, 0, NULL, 0, 1, N'Canada', @festivalUserId, @now);

SELECT @catFilm = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Narrative Film';
SELECT @catPhoto = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Photography Portfolio';
SELECT @catMusic = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Original Music';
SELECT @catScript = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Screenwriting';
SELECT @catXr = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Immersive XR';
SELECT @catArt = [Id] FROM [hisubmi1_user].[EventCategories] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Visual Art';

DECLARE @deadlineEarly int;
DECLARE @deadlineStandard int;
DECLARE @deadlineLate int;

INSERT INTO [hisubmi1_user].[DeadLines]
    ([Name], [Date], [ApplyToAllCategory], [FestivalId], [CreatedBy], [CreatedOn])
VALUES
    (N'QA Flash Deadline - 19 August 2026', '2026-08-19', 1, @festivalId, @festivalUserId, @now),
    (N'QA Regular Deadline', '2026-09-20', 1, @festivalId, @festivalUserId, @now),
    (N'QA Final Deadline', '2026-10-05', 1, @festivalId, @festivalUserId, @now);

SELECT @deadlineEarly = [Id] FROM [hisubmi1_user].[DeadLines] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Flash Deadline - 19 August 2026';
SELECT @deadlineStandard = [Id] FROM [hisubmi1_user].[DeadLines] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Regular Deadline';
SELECT @deadlineLate = [Id] FROM [hisubmi1_user].[DeadLines] WHERE [FestivalId] = @festivalId AND [Name] = N'QA Final Deadline';

INSERT INTO [hisubmi1_user].[DeadlineEventCategories]
(
    [GoldFee], [StudentFee], [StandardFee], [EventCategoryId], [DeadLineId],
    [CreatedBy], [CreatedOn]
)
SELECT 40, 15, 25, C.[Id], D.[Id], @festivalUserId, @now
FROM (VALUES (@catFilm),(@catPhoto),(@catMusic),(@catScript),(@catXr),(@catArt)) C([Id])
JOIN (VALUES (@deadlineEarly),(@deadlineStandard),(@deadlineLate)) D([Id]) ON 1 = 1;

INSERT INTO [hisubmi1_user].[SubmissionQuestions]
(
    [Title], [Questiontype], [FestivalId], [ApplyforAllCategory],
    [CreatedBy], [CreatedOn]
)
VALUES
    (N'What should the jury know about this work?', 5, @festivalId, 1, @festivalUserId, @now),
    (N'Confirm that you own or control the rights to this submission.', 4, @festivalId, 1, @festivalUserId, @now),
    (N'What is the preferred screening or display format?', 1, @festivalId, 1, @festivalUserId, @now);

INSERT INTO [hisubmi1_user].[FestivalArtCategories]
    ([FestivalId], [ArtCategoryId], [CreatedBy], [CreatedOn])
VALUES
    (@festivalId, 5, @festivalUserId, @now),
    (@festivalId, 6, @festivalUserId, @now),
    (@festivalId, 15, @festivalUserId, @now);

INSERT INTO [hisubmi1_user].[FestivalFestivalFoci]
    ([FestivalId], [FestivalFocusId], [CreatedBy], [CreatedOn])
VALUES
    (@festivalId, 5, @festivalUserId, @now),
    (@festivalId, 6, @festivalUserId, @now),
    (@festivalId, 11, @festivalUserId, @now);

INSERT INTO [hisubmi1_user].[FestivalFestivalQualifyings]
    ([FestivalId], [FestivalQualifyingId], [CreatedBy], [CreatedOn])
VALUES
    (@festivalId, 1, @festivalUserId, @now),
    (@festivalId, 2, @festivalUserId, @now),
    (@festivalId, 3, @festivalUserId, @now);

DECLARE @filmId int;
DECLARE @photoId int;
DECLARE @musicId int;
DECLARE @scriptId int;
DECLARE @xrId int;
DECLARE @artId int;

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Lanterns Between Tides', N'Film sample with complete metadata', 1, 0,
    N'A quiet short film about memory, water and the small rituals that connect generations.',
    N'Lanterns Between Tides', N'Original synopsis: a coastal archivist follows a trail of handmade lanterns through one changing night.',
    N'https://example.test/lanterns-between-tides', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 1128375, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/media/qa/qa-flower.mp4', N'/media/qa/qa-flower.mp4', NULL,
    N'Main film preview for testing video playback.', N'qa-lanterns-between-tides-2026',
    0, NULL, NULL, 2, @artistId, @now
);
SET @filmId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Geometry of Quiet Light', N'Photography series with camera metadata', 2, 0,
    N'A photographic study of light crossing modern architecture during one winter afternoon.',
    N'Geometry of Quiet Light', N'Original synopsis: six frames observe the relationship between concrete, shadow and human scale.',
    N'https://example.test/geometry-of-quiet-light', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 2048, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/img/1.jpg', N'/img/1.jpg', NULL, N'Main photography image for gallery testing.',
    N'qa-geometry-of-quiet-light-2026', 1, N'OCAD University', N'/img/2.png', 2,
    @artistId, @now
);
SET @photoId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Field Notes for the Moon', N'Music sample with playable audio', 3, 0,
    N'An ambient composition combining prepared piano, field recordings and a restrained melodic line.',
    N'Field Notes for the Moon', N'Original synopsis: a nocturnal sound sketch written for a small gallery installation.',
    N'https://example.test/field-notes-for-the-moon', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 356000, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/media/notification.mp3', N'/media/notification.mp3', NULL,
    N'Audio preview for testing the music player.', N'qa-field-notes-for-the-moon-2026',
    0, NULL, NULL, 2, @artistId, @now
);
SET @musicId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] The Last Archive', N'Screenplay sample with PDF preview', 4, 0,
    N'A near-future drama about a librarian protecting the final physical archive in a city that has forgotten how to read.',
    N'The Last Archive', N'Original synopsis: an archivist must choose between saving a collection and saving the person who created it.',
    N'https://example.test/the-last-archive', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 13264, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/media/qa/qa-script.pdf', N'/media/qa/qa-script.pdf', NULL,
    N'PDF screenplay preview for testing document rendering.', N'qa-the-last-archive-2026',
    0, NULL, NULL, 2, @artistId, @now
);
SET @scriptId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Breathing Room 360', N'Immersive VR/XR sample with video preview', 5, 0,
    N'An immersive meditation room that responds to movement, light and the viewer''s breathing rhythm.',
    N'Breathing Room 360', N'Original synopsis: a responsive virtual room turns attention into a navigable landscape.',
    N'https://example.test/breathing-room-360', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 1128375, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/media/qa/qa-flower.mp4', N'/media/qa/qa-flower.mp4', NULL,
    N'Video preview for testing immersive project playback.', N'qa-breathing-room-360-2026',
    1, N'OCAD University', N'/img/2.png', 2, @artistId, @now
);
SET @xrId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [OriginalTitle], [OriginalBriefSynopsis],
    [WebSite], [Twitter], [Youtube], [Instagram], [Telegram], [WhatsApp],
    [Size], [UseCurrentUserInformation], [UserId], [Email], [PhoneNumber],
    [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [FileURl], [LocalFileURL], [Password], [FileDescription], [URL],
    [StudentProject], [UniversityName], [StudentPhotoCard], [ProjectStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[QA] Soft Machinery', N'Mixed-media visual art installation', 6, 0,
    N'A mixed-media installation about the emotional life of everyday machines and the traces left by their users.',
    N'Soft Machinery', N'Original synopsis: found materials, light and layered images form a quiet room of remembered routines.',
    N'https://example.test/soft-machinery', N'https://twitter.com/', N'https://youtube.com/',
    N'https://instagram.com/', N'https://t.me/', N'+1 416 555 2026', 4096, 1, @artistId,
    N'qa.artist@hisubmit.test', N'+1 416 555 2026', N'QA', N'Artist', '1992-04-18', 2, 1,
    N'/img/ArtWall.jpg', N'/img/ArtWall.jpg', NULL,
    N'Primary art image and installation preview.', N'qa-soft-machinery-2026',
    0, NULL, NULL, 2, @artistId, @now
);
SET @artId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[FilmSpecification]
(
    [Genre], [RunTimeHours], [RunTimeMinutes], [RunTimeSecounds],
    [CompletionDate], [MonetaryUnitId], [ProductionBudget], [OriginCountryId],
    [Language], [ShottingFormat], [AspectRatio], [FilmColor],
    [StudentProject], [FirstTimeFilmMaker], [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Drama / Experimental', 0, 14, 32, '2026-06-01', 1, 18000, 44,
    N'English', N'Digital Cinema 4K', N'2.39:1', 0, 0, 1,
    @filmId, @artistId, @now
);

INSERT INTO [hisubmi1_user].[PhotographySpecifications]
(
    [Genre], [TakenDate], [OriginCountryId], [Camera], [Lens],
    [FocalLength], [Location], [ShutterSpeed], [Aperture], [Iso_Film],
    [StudentProject], [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Architecture / Fine Art', '2026-02-14', 44, N'Sony A7R IV',
    N'FE 24-70mm F2.8 GM', N'50mm', N'Toronto, Ontario', N'1/250 sec',
    N'f/5.6', N'ISO 100', 1, @photoId, @artistId, @now
);

INSERT INTO [hisubmi1_user].[MusicSpecifications]
(
    [Genre], [RunTimeHours], [RunTimeMinutes], [RunTimeSecounds],
    [CompletionDate], [OriginCountryId], [Language], [StudentProject],
    [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Ambient / Contemporary', 0, 3, 48, '2026-05-20', 44, N'Instrumental',
    0, @musicId, @artistId, @now
);

INSERT INTO [hisubmi1_user].[ScriptSpecifications]
(
    [Genre], [NumberOfPage], [OriginCountryId], [Language],
    [StudentProject], [FirstTimeScreenWrite], [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Science fiction drama', 108, 44, N'English', 0, 0, @scriptId, @artistId, @now
);

INSERT INTO [hisubmi1_user].[XrVrSpecifications]
(
    [Genre], [RunTimeHours], [RunTimeMinutes], [RunTimeSecounds],
    [VariableRunTime], [DescriptionRunTime], [MinRunTimeHours],
    [MinRunTimeMinutes], [MinRunTimeSecounds], [MaxTimeHours],
    [MaxTimeMinutes], [MaxTimeSecounds], [AvgTimeHours],
    [AvgTimeMinutes], [AvgTimeSecounds], [CompletionDate],
    [MonetaryUnitId], [ProductionBudget], [OriginCountryId], [Language],
    [StudentProject], [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'Immersive installation', 0, 12, 0, 1,
    N'Variable session length; visitors can leave at any time.',
    0, 5, 0, 0, 30, 0, 0, 12, 0, '2026-07-10', 1, 25000, 44,
    N'English', 1, @xrId, @artistId, @now
);

DECLARE @fileId int;

INSERT INTO [hisubmi1_user].[ProjectFile]
(
    [Name], [FileURL], [IsLocalFile], [IsMainFile], [LocalFileURL],
    [FileFormat], [ProjectId], [Order], [FileDescription], [Position],
    [CreatedBy], [CreatedOn]
)
VALUES
    (N'Film preview MP4', N'/media/qa/qa-flower.mp4', 1, 1, N'/media/qa/qa-flower.mp4', 1, @filmId, 1, N'Playable film preview.', 0, @artistId, @now),
    (N'Photography primary image', N'/img/1.jpg', 1, 1, N'/img/1.jpg', 0, @photoId, 1, N'Primary photography image.', 0, @artistId, @now),
    (N'Music preview MP3', N'/media/notification.mp3', 1, 1, N'/media/notification.mp3', 3, @musicId, 1, N'Playable music preview.', 0, @artistId, @now),
    (N'Screenplay PDF', N'/media/qa/qa-script.pdf', 1, 1, N'/media/qa/qa-script.pdf', 6, @scriptId, 1, N'Readable screenplay preview.', 0, @artistId, @now),
    (N'XR video preview', N'/media/qa/qa-flower.mp4', 1, 1, N'/media/qa/qa-flower.mp4', 1, @xrId, 1, N'Immersive project preview.', 0, @artistId, @now),
    (N'Art installation image', N'/img/ArtWall.jpg', 1, 1, N'/img/ArtWall.jpg', 0, @artId, 1, N'Primary visual art image.', 0, @artistId, @now);

INSERT INTO [hisubmi1_user].[ProjectImages]
(
    [Url], [Title], [Location], [State], [ProjectId], [CreatedBy], [CreatedOn]
)
VALUES
    (N'/img/1.jpg', N'Architecture study detail', N'Toronto', N'Ontario', @photoId, @artistId, @now),
    (N'/img/2.png', N'Light and shadow detail', N'Toronto', N'Ontario', @photoId, @artistId, @now),
    (N'/img/ArtWall.jpg', N'Soft Machinery installation view', N'Toronto', N'Ontario', @artId, @artistId, @now),
    (N'/img/1.jpg', N'Lanterns visual still', N'Toronto', N'Ontario', @filmId, @artistId, @now),
    (N'/img/2.png', N'Breathing Room visual still', N'Toronto', N'Ontario', @xrId, @artistId, @now);

DECLARE @creditId int;
INSERT INTO [hisubmi1_user].[ProjectCredits]
    ([Title], [ProjectId], [ImageUrl], [CreatedBy], [CreatedOn])
VALUES
    (N'Creative team', @filmId, N'/assets/img/avatar.png', @artistId, @now);
SET @creditId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[ProjectItemPeople]
    ([Name], [Email], [PriorCredit], [ProjectCreditId], [CreatedBy], [CreatedOn])
VALUES
    (N'QA Artist', N'qa.artist@hisubmit.test', N'Writer / Director', @creditId, @artistId, @now);

INSERT INTO [hisubmi1_user].[ProjectCredits]
    ([Title], [ProjectId], [ImageUrl], [CreatedBy], [CreatedOn])
VALUES
    (N'Artist', @photoId, N'/assets/img/avatar.png', @artistId, @now),
    (N'Composer', @musicId, N'/assets/img/avatar.png', @artistId, @now),
    (N'Writer', @scriptId, N'/assets/img/avatar.png', @artistId, @now),
    (N'Experience designer', @xrId, N'/assets/img/avatar.png', @artistId, @now),
    (N'Artist / installation designer', @artId, N'/assets/img/avatar.png', @artistId, @now);

INSERT INTO [hisubmi1_user].[Awards]
    ([Title], [Location], [AwardsWon], [Date], [ImageUrl], [ProjectId], [CreatedBy], [CreatedOn])
SELECT N'QA Jury Selection', N'Toronto', N'Official Selection', '2026-07-15', N'/img/FestivalQualifying/academy.png', P.[Id], @artistId, @now
FROM (VALUES (@filmId),(@photoId),(@musicId),(@scriptId),(@xrId),(@artId)) P([Id]);

INSERT INTO [hisubmi1_user].[ScreeningAwards]
    ([ScreeningDate], [City], [CountryId], [Premiere], [AwardSelection], [Title],
     [ProjectId], [ImageUrl], [CreatedBy], [CreatedOn])
SELECT '2026-10-25', N'Toronto', 44, N'Canadian Premiere', N'QA Showcase Selection',
       N'Complete Creative Showcase', P.[Id], N'/img/FestivalQualifying/academy.png',
       @artistId, @now
FROM (VALUES (@filmId),(@photoId),(@musicId),(@scriptId),(@xrId),(@artId)) P([Id]);

DECLARE @distributionId int;
INSERT INTO [hisubmi1_user].[DistributionInformation]
    ([Title], [ProjectId], [DistributionType], [CreatedBy], [CreatedOn])
VALUES
    (N'North America festival distribution', @filmId, 1, @artistId, @now);
SET @distributionId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[DistributionInformationItems]
    ([CountryId], [DistributionInformationId], [CreatedBy], [CreatedOn])
VALUES
    (44, @distributionId, @artistId, @now),
    (210, @distributionId, @artistId, @now);

INSERT INTO [hisubmi1_user].[Addresses]
(
    [Text], [City], [State], [PostalCode], [CountryId], [ProjectId],
    [MapLocation], [CreatedBy], [CreatedOn]
)
SELECT N'QA Artist Studio, 42 Dundas Street', N'Toronto', N'Ontario',
       N'M5G 1Z3', 44, P.[Id], N'43.6561,-79.3802', @artistId, @now
FROM (VALUES (@filmId),(@photoId),(@musicId),(@scriptId),(@xrId),(@artId)) P([Id]);

INSERT INTO [hisubmi1_user].[Submits]
(
    [FestivalId], [ProjectId], [SubmitDate], [SubmitStatus], [JudgingStatus],
    [Comment], [TrackingCode], [CreatedBy], [CreatedOn]
)
SELECT @festivalId, P.[Id], @now, 2, 3,
       N'QA submission accepted for judging and display validation.',
       CONCAT(N'QA26-', P.[Id]), @artistId, @now
FROM (VALUES (@filmId),(@photoId),(@musicId),(@scriptId),(@xrId),(@artId)) P([Id]);

UPDATE [hisubmi1_user].[Festivals]
SET [MinFee] = 15, [MaxFee] = 40
WHERE [Id] = @festivalId;

COMMIT TRANSACTION;

SELECT
    N'QA showcase seed completed' AS [Result],
    @artistId AS [ArtistUserId],
    @festivalUserId AS [FestivalUserId],
    @refereeId AS [RefereeUserId],
    @festivalId AS [FestivalId],
    @filmId AS [FilmProjectId],
    @photoId AS [PhotographyProjectId],
    @musicId AS [MusicProjectId],
    @scriptId AS [ScriptProjectId],
    @xrId AS [XrProjectId],
    @artId AS [ArtProjectId];
