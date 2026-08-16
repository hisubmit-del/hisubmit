/*
    HiSubmit local demo data seed
    Target: HiSubmitDB50 on the local SQL Server only.

    This script is intentionally not called by the application startup.
    It is idempotent by marker: if the first demo festival already exists,
    the transaction exits without changing anything.

    Demo login password for all accounts:
    123Pa$$word!
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
    WHERE [Name] = N'[DEMO] Aurora Arts & Film Festival'
)
BEGIN
    PRINT 'Demo seed already exists. No changes were made.';
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
    THROW 51000, 'The seeded local user johndoe was not found; aborting demo seed.', 1;

DECLARE @artistId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
DECLARE @festivalManagerId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
DECLARE @refereeId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
DECLARE @productBuyerId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
DECLARE @ticketBuyerId nvarchar(450) = CONVERT(nvarchar(450), NEWID());

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
    THROW 51001, 'Required Artist/Festival roles were not found; aborting demo seed.', 1;

INSERT INTO [Identity].[Users]
(
    [Id], [VerificationCode], [FirstName], [LastName], [CreatedBy],
    [ProfilePictureDataUrl], [CreatedOn], [LastModifiedBy], [LastModifiedOn],
    [IsDeleted], [DeletedOn], [IsActive], [RefreshToken],
    [RefreshTokenExpiryTime], [FeeStatus], [FeeStatusExpirationDate],
    [UserName], [NormalizedUserName], [Email], [NormalizedEmail],
    [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
    [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd],
    [LockoutEnabled], [AccessFailedCount]
)
VALUES
(
    @artistId, NULL, N'Demo', N'Artist', NULL, NULL, @now, NULL, NULL,
    0, NULL, 1, NULL, '0001-01-01', 0, NULL,
    N'demo.artist', N'DEMO.ARTIST', N'demo.artist@hisubmit.test',
    N'DEMO.ARTIST@HISUBMIT.TEST', 1, @passwordHash, CONVERT(nvarchar(450), NEWID()),
    CONVERT(nvarchar(450), NEWID()), NULL, 1, 0, NULL, 1, 0
),
(
    @festivalManagerId, NULL, N'Demo', N'Festival Manager', NULL, NULL, @now, NULL, NULL,
    0, NULL, 1, NULL, '0001-01-01', 0, NULL,
    N'demo.festival', N'DEMO.FESTIVAL', N'demo.festival@hisubmit.test',
    N'DEMO.FESTIVAL@HISUBMIT.TEST', 1, @passwordHash, CONVERT(nvarchar(450), NEWID()),
    CONVERT(nvarchar(450), NEWID()), NULL, 1, 0, NULL, 1, 0
),
(
    @refereeId, NULL, N'Demo', N'Referee', NULL, NULL, @now, NULL, NULL,
    0, NULL, 1, NULL, '0001-01-01', 0, NULL,
    N'demo.referee', N'DEMO.REFEREE', N'demo.referee@hisubmit.test',
    N'DEMO.REFEREE@HISUBMIT.TEST', 1, @passwordHash, CONVERT(nvarchar(450), NEWID()),
    CONVERT(nvarchar(450), NEWID()), NULL, 1, 0, NULL, 1, 0
),
(
    @productBuyerId, NULL, N'Demo', N'Product Buyer', NULL, NULL, @now, NULL, NULL,
    0, NULL, 1, NULL, '0001-01-01', 0, NULL,
    N'demo.productbuyer', N'DEMO.PRODUCTBUYER', N'demo.productbuyer@hisubmit.test',
    N'DEMO.PRODUCTBUYER@HISUBMIT.TEST', 1, @passwordHash, CONVERT(nvarchar(450), NEWID()),
    CONVERT(nvarchar(450), NEWID()), NULL, 1, 0, NULL, 1, 0
),
(
    @ticketBuyerId, NULL, N'Demo', N'Ticket Buyer', NULL, NULL, @now, NULL, NULL,
    0, NULL, 1, NULL, '0001-01-01', 0, NULL,
    N'demo.ticketbuyer', N'DEMO.TICKETBUYER', N'demo.ticketbuyer@hisubmit.test',
    N'DEMO.TICKETBUYER@HISUBMIT.TEST', 1, @passwordHash, CONVERT(nvarchar(450), NEWID()),
    CONVERT(nvarchar(450), NEWID()), NULL, 1, 0, NULL, 1, 0
);

INSERT INTO [Identity].[UserRoles] ([UserId], [RoleId])
VALUES
    (@artistId, @artistRoleId),
    (@festivalManagerId, @festivalRoleId),
    (@refereeId, @artistRoleId),
    (@productBuyerId, @artistRoleId),
    (@ticketBuyerId, @artistRoleId);

DECLARE @masterOneId int;
DECLARE @masterTwoId int;
DECLARE @festivalOneId int;
DECLARE @festivalTwoId int;

INSERT INTO [hisubmi1_user].[FestivalMasters]
    ([Name], [ActivePeriod], [ActiveId], [UserId], [CreatedBy], [CreatedOn])
VALUES
    (N'[DEMO] Aurora Arts Master', 0, 0, @festivalManagerId, @festivalManagerId, @now);
SET @masterOneId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[FestivalMasters]
    ([Name], [ActivePeriod], [ActiveId], [UserId], [CreatedBy], [CreatedOn])
VALUES
    (N'[DEMO] Northlight Shorts Master', 0, 0, @festivalManagerId, @festivalManagerId, @now);
SET @masterTwoId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Festivals]
(
    [FestivalMasterId], [Name], [UserId], [Description], [LogoURL], [Rules],
    [YearsRunning], [AudienceAttendence], [EstimatedSubmissions],
    [ProjectsSelected], [AwardsPresented], [EventType], [FilmFestival],
    [ScreenWritingWriter], [MusicContest], [PhotographicContest],
    [OnlineFestival], [ArtFestival], [WebSite], [Email], [SeparateSubmissiionAddress],
    [OnlineEvent], [OpeningDate], [NotificationDate], [EventStartDate],
    [EventEndDate], [Public], [SearchTerms], [AllLenghtAccepted],
    [URL], [StartingNumber], [IsActive], [FestivalStatus], [FeeStatus],
    [IsActivePeriod], [CreatedBy], [CreatedOn]
)
VALUES
(
    @masterOneId, N'[DEMO] Aurora Arts & Film Festival', @festivalManagerId,
    N'Demo festival for testing artist submissions, judging, products, tickets and news.',
    N'/img/ArtWall.jpg', N'Demo rules: all submitted work must be original.',
    4, 350, 120, 30, 6, 1, 1, 0, 0, 0, 0, 0,
    N'https://example.test/aurora', N'demo.festival@hisubmit.test', 0, 1,
    '2026-08-01', '2026-10-01', '2026-10-15', '2026-10-20', 1,
    N'demo,film,short,arts', 1, N'demo-aurora-arts-film-2026', 1000, 1, 2, 0,
    1, @festivalManagerId, @now
);
SET @festivalOneId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Festivals]
(
    [FestivalMasterId], [Name], [UserId], [Description], [Rules], [YearsRunning],
    [AudienceAttendence], [EstimatedSubmissions], [ProjectsSelected],
    [AwardsPresented], [EventType], [FilmFestival], [ScreenWritingWriter],
    [MusicContest], [PhotographicContest], [OnlineFestival], [ArtFestival],
    [SeparateSubmissiionAddress], [OnlineEvent], [OpeningDate],
    [NotificationDate], [EventStartDate], [EventEndDate], [Public],
    [SearchTerms], [AllLenghtAccepted], [URL], [StartingNumber], [IsActive],
    [FestivalStatus], [FeeStatus], [IsActivePeriod], [CreatedBy], [CreatedOn]
)
VALUES
(
    @masterTwoId, N'[DEMO] Northlight Short Film Week', @festivalManagerId,
    N'Demo online and in-person short film event used for regression testing.',
    N'Demo rules for short films.', 2, 180, 80, 18, 4, 1, 1, 0, 0, 0, 1, 0,
    0, 1, '2026-08-01', '2026-11-01', '2026-11-15', '2026-11-18', 1,
    N'demo,short film,online festival', 1, N'demo-northlight-shorts-2026', 2000,
    1, 2, 0, 1, @festivalManagerId, @now
);
SET @festivalTwoId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @refereeRoleId nvarchar(450) = CONVERT(nvarchar(450), NEWID());
INSERT INTO [Identity].[Roles]
(
    [Id], [Description], [CreatedBy], [CreatedOn],
    [Name], [NormalizedName], [ConcurrencyStamp], [FestivalId]
)
VALUES
(
    @refereeRoleId, N'Demo referee role', @festivalManagerId, @now,
    N'Referee', N'REFEREE', CONVERT(nvarchar(450), NEWID()), @festivalOneId
);

INSERT INTO [Identity].[UserRoles] ([UserId], [RoleId])
VALUES (@refereeId, @refereeRoleId);

INSERT INTO [hisubmi1_user].[FestivalSubUser]
    ([FestivalId], [UserId], [IsReferee], [IsRemoved], [CreatedBy], [CreatedOn])
VALUES
    (@festivalOneId, @festivalManagerId, 0, 0, @festivalManagerId, @now),
    (@festivalOneId, @refereeId, 1, 0, @festivalManagerId, @now);

DECLARE @categoryOneId int;
DECLARE @categoryTwoId int;
INSERT INTO [hisubmi1_user].[EventCategories]
(
    [Name], [Description], [FestivalId], [ProjectType], [FirstRunTimeValue],
    [RequirePassword], [StudentProject], [CreatedBy], [CreatedOn]
)
VALUES
    (N'DEMO Short Film', N'Demo short film category.', @festivalOneId, 1, 1, 0, 0, @festivalManagerId, @now),
    (N'DEMO Student Film', N'Demo student category.', @festivalOneId, 1, 1, 0, 1, @festivalManagerId, @now);
SELECT @categoryOneId = [Id]
FROM [hisubmi1_user].[EventCategories]
WHERE [FestivalId] = @festivalOneId AND [Name] = N'DEMO Short Film';
SELECT @categoryTwoId = [Id]
FROM [hisubmi1_user].[EventCategories]
WHERE [FestivalId] = @festivalOneId AND [Name] = N'DEMO Student Film';

DECLARE @deadlineId int;
INSERT INTO [hisubmi1_user].[DeadLines]
    ([Name], [Date], [ApplyToAllCategory], [FestivalId], [CreatedBy], [CreatedOn])
VALUES
    (N'DEMO Early Deadline', '2026-09-15', 1, @festivalOneId, @festivalManagerId, @now);
SET @deadlineId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[DeadlineEventCategories]
    ([GoldFee], [StudentFee], [StandardFee], [EventCategoryId], [DeadLineId], [CreatedBy], [CreatedOn])
VALUES
    (25, 10, 18, @categoryOneId, @deadlineId, @festivalManagerId, @now),
    (20, 8, 15, @categoryTwoId, @deadlineId, @festivalManagerId, @now);

DECLARE @productId int;
INSERT INTO [hisubmi1_user].[Products]
(
    [Name], [ShortDescription], [Description], [Price], [ProductType],
    [IsEnable], [FestivalId], [CreatedBy], [CreatedOn], [Status]
)
VALUES
(
    N'[DEMO] Aurora Festival Poster', N'Digital demo poster',
    N'Demo downloadable festival poster for checkout testing.', 12.50, 0, 1,
    @festivalOneId, @festivalManagerId, @now, 0
);
SET @productId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[News]
(
    [Title], [BannerUrl], [Description], [IsEnable], [ShortDescription],
    [FestivalId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[DEMO] Aurora festival opens submissions',
    N'/img/article-pattern.png',
    N'<p>This is demo news for testing the festival publisher workflow.</p>',
    1, N'Demo submission announcement.', @festivalOneId, @festivalManagerId, @now
),
(
    N'[DEMO] Northlight announces its program',
    N'/img/ArtWall.jpg',
    N'<p>This is demo news for testing public festival news.</p>',
    1, N'Demo program announcement.', @festivalTwoId, @festivalManagerId, @now
);

DECLARE @ticketId int;
DECLARE @ticketAddressId int;
DECLARE @venueId int;
INSERT INTO [hisubmi1_user].[Addresses]
(
    [Text], [City], [State], [PostalCode], [CountryId],
    [FestivalId], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'DEMO Aurora Arts Center', N'Toronto', N'Ontario', N'M5V 1A1', 2,
    @festivalOneId, @festivalManagerId, @now
);
SET @ticketAddressId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Venues]
(
    [Name], [FestivalId], [AddressId], [VenueType],
    [Capacity], [AvailableCapacity], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[DEMO] Aurora Arts Center', @festivalOneId, @ticketAddressId, 1,
    100, 99, @festivalManagerId, @now
);
SET @venueId = CONVERT(int, SCOPE_IDENTITY());
UPDATE [hisubmi1_user].[Addresses]
SET [VenueId] = @venueId
WHERE [Id] = @ticketAddressId;

INSERT INTO [hisubmi1_user].[Tickets]
(
    [Title], [Description], [OpenDate], [CloseDate], [AddManagerPercentage],
    [Cost], [VenueId], [EventDate], [TicketType], [IsEnable], [Capacity],
    [AvailableCapacity], [CreatedBy], [CreatedOn], [Status]
)
VALUES
(
    N'[DEMO] Aurora Opening Night', N'Demo ticket for ticket checkout testing.',
    '2026-08-15', '2026-10-14', 0, 20, @venueId, '2026-10-15', 0, 1, 100, 99,
    @festivalManagerId, @now, 0
);
SET @ticketId = CONVERT(int, SCOPE_IDENTITY());

DECLARE @projectId int;
INSERT INTO [hisubmi1_user].[Projects]
(
    [Title], [SubTitle], [ProjectType], [HasNoneEnglishTitle],
    [EnglishBriefSynopsis], [Size], [UseCurrentUserInformation], [UserId],
    [Email], [FirstName], [LastName], [BirthDate], [Gender], [IsLocalFile],
    [URL], [StudentProject], [ProjectStatus], [CreatedBy], [CreatedOn]
)
VALUES
(
    N'[DEMO] The Paper Moon', N'Demo artist project', 1, 0,
    N'A demo submission used to test artist and referee workflows.', 0, 1,
    @artistId, N'demo.artist@hisubmit.test', N'Demo', N'Artist',
    '1995-01-01', 0, 0, N'demo-the-paper-moon', 0, 0, @artistId, @now
);
SET @projectId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[Submits]
(
    [FestivalId], [ProjectId], [SubmitDate], [SubmitStatus], [JudgingStatus],
    [Comment], [TrackingCode], [CreatedBy], [CreatedOn]
)
VALUES
(
    @festivalOneId, @projectId, @now, 2, 0,
    N'Demo submission awaiting referee review.', N'DEMO-AURORA-0001',
    @artistId, @now
);

DECLARE @productCartId int;
INSERT INTO [hisubmi1_user].[Carts]
(
    [Paid], [Price], [UserId], [OrderId], [PaymentId], [PayerId], [Email],
    [CartDate], [CreatedBy], [CreatedOn]
)
VALUES
(
    1, 12.50, @productBuyerId, N'DEMO-PRODUCT-ORDER-001',
    N'DEMO-PRODUCT-PAYMENT-001', @productBuyerId,
    N'demo.productbuyer@hisubmit.test', @now, @productBuyerId, @now
);
SET @productCartId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[ProductSold]
(
    [UserId], [Email], [ProductId], [Income], [ShareFestivalIncome],
    [Status], [CreatedBy], [CreatedOn]
)
VALUES
(
    @productBuyerId, N'demo.productbuyer@hisubmit.test', @productId,
    12.50, 1.25, 2, @productBuyerId, @now
);

INSERT INTO [hisubmi1_user].[CarTItems]
(
    [Title], [ItemId], [Price], [Description], [ImageUrl], [CartId],
    [CartItemType], [CreatedBy], [CreatedOn], [PriceAfterDiscount]
)
VALUES
(
    N'[DEMO] Aurora Festival Poster', CONVERT(nvarchar(max), @productId),
    12.50, N'Demo purchased product', N'/img/ArtWall.jpg', @productCartId,
    6, @productBuyerId, @now, 12.50
);

DECLARE @ticketCartId int;
INSERT INTO [hisubmi1_user].[Carts]
(
    [Paid], [Price], [UserId], [OrderId], [PaymentId], [PayerId], [Email],
    [CartDate], [CreatedBy], [CreatedOn]
)
VALUES
(
    1, 20, @ticketBuyerId, N'DEMO-TICKET-ORDER-001',
    N'DEMO-TICKET-PAYMENT-001', @ticketBuyerId,
    N'demo.ticketbuyer@hisubmit.test', @now, @ticketBuyerId, @now
);
SET @ticketCartId = CONVERT(int, SCOPE_IDENTITY());

INSERT INTO [hisubmi1_user].[SealedTickets]
(
    [Cost], [ShareFestivalIncome], [Count], [SerialNumber], [BuyDate],
    [UserId], [TicketId], [ForOtherUser], [SoldTicketStatus],
    [CreatedBy], [CreatedOn]
)
VALUES
(
    20, 2, 1, NEWID(), @now, @ticketBuyerId, @ticketId, 0, 2,
    @ticketBuyerId, @now
);

INSERT INTO [hisubmi1_user].[CarTItems]
(
    [Title], [ItemId], [Price], [Description], [ImageUrl], [CartId],
    [SoldTicketId], [CartItemType], [CreatedBy], [CreatedOn],
    [PriceAfterDiscount]
)
VALUES
(
    N'[DEMO] Aurora Opening Night', CONVERT(nvarchar(max), @ticketId),
    20, N'Demo purchased ticket', NULL, @ticketCartId,
    CONVERT(int, SCOPE_IDENTITY()), 3, @ticketBuyerId, @now, 20
);

COMMIT TRANSACTION;

SELECT
    N'DEMO seed completed' AS Result,
    @artistId AS ArtistUserId,
    @festivalManagerId AS FestivalManagerUserId,
    @refereeId AS RefereeUserId,
    @productBuyerId AS ProductBuyerUserId,
    @ticketBuyerId AS TicketBuyerUserId,
    @festivalOneId AS PrimaryFestivalId,
    @festivalTwoId AS SecondaryFestivalId;
