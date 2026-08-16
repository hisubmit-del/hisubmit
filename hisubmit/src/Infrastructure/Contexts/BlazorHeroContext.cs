using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Domain.Entities.ExtendedAttributes;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Entities.Misc;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities;
using HiSubmit.Domain.Entities.Chats;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Entities.Files;
using HiSubmit.Domain.Entities.SeoTags;

namespace HiSubmit.Infrastructure.Contexts;

public class BlazorHeroContext : AuditableContext
{
    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public BlazorHeroContext(DbContextOptions<BlazorHeroContext> options, ICurrentUserService currentUserService,
        IDateTimeService dateTimeService)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeService = dateTimeService;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSold> SoldProducts { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<DocumentType> DocumentTypes { get; set; }
    public DbSet<ChatHistory<BlazorHeroUser>> ChatHistories { get; set; }
    public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }

    #region FestivalId

    public DbSet<Image> Images { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Judging> Judgings { get; set; }
    public DbSet<Festival> Festivals { get; set; }
    public DbSet<DeadLine> DeadLines { get; set; }
    public DbSet<ArtCategory> ArtCategories { get; set; }
    public DbSet<FestivalFocus> FestivalFoci { get; set; }
    public DbSet<JudgingFiled> JudgingFileds { get; set; }
    public DbSet<JudgingButton> JudgingButtons { get; set; }
    public DbSet<EventCategory> EventCategories { get; set; }
    public DbSet<FestivalMaster> FestivalMasters { get; set; }
    public DbSet<EventOrginizer> EventOrginizers { get; set; }
    public DbSet<FestivalFile> FestivalFiles { get; set; }
    public DbSet<ProjectJudging> ProjectJudgings { get; set; }
    public DbSet<FestivalSubUser> FestivalSubUser { get; set; }
    public DbSet<FestivalQualifying> FestivalQualifyings { get; set; }
    public DbSet<SubmissionQuestion> SubmissionQuestions { get; set; }
    public DbSet<FestivalArtCategory> FestivalArtCategories { get; set; }
    public DbSet<FestivalFestivalFocus> FestivalFestivalFoci { get; set; }
    public DbSet<DeadlineEventCategory> DeadlineEventCategories { get; set; }
    public DbSet<DropDownOptionCheckBoxItem> DropDownOptionCheckBoxItems { get; set; }
    public DbSet<FestivalFestivalQualifying> FestivalFestivalQualifyings { get; set; }
    public DbSet<SubmissionQuestionEventCategory> SubmissionQuestionEventCategories { get; set; }

    public DbSet<Like> FestivalLikes { get; set; }
    #region Chats

    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }

    #endregion

    #endregion

    #region Location

    public DbSet<Country> Countries { get; set; }
    public DbSet<Address> Addresses { get; set; }

    #endregion

    #region Submit

    public DbSet<Submit> Submits { get; set; }
    public DbSet<SubmitDeadLineCategories> SubmitDeadLineCategories { get; set; }

    #endregion

    #region Payments

    public DbSet<Cart> Carts { get; set; }
    public DbSet<CarTItem> CarTItems { get; set; }
    public DbSet<DiscountCode> DiscountCodes { get; set; }
    public DbSet<ProductSold> ProductsSold { get; set; }
    public DbSet<SiteCommission> SiteCommissions { get; set; }
    public DbSet<UserSpecialPeriod> UserSpecialPeriods { get; set; }
    public DbSet<FestivalPaymentItem> FestivalPaymentItems { get; set; }
    public DbSet<FestivalPaymentInformation> FestivalPaymentsInformation { get; set; }

    #endregion

    #region Projects

    public DbSet<Award> Awards { get; set; }
    public DbSet<DistributionInformation> DistributionInformation { get; set; }
    public DbSet<DistributionInformationItem> DistributionInformationItems { get; set; }
    public DbSet<FilmSpecification> FilmSpecification { get; set; }
    public DbSet<MediaRight> MediaRights { get; set; }
    public DbSet<MediaRightDistributionInformation> MediaRightDistributionInformation { get; set; }
    public DbSet<MonetaryUnit> MonetaryUnits { get; set; }
    public DbSet<MusicSpecification> MusicSpecifications { get; set; }
    public DbSet<PhotographySpecification> PhotographySpecifications { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectCredit> ProjectCredits { get; set; }
    public DbSet<ProjectItemPerson> ProjectItemPeople { get; set; }
    public DbSet<ScreeningAward> ScreeningAwards { get; set; }
    public DbSet<ScriptSpecification> ScriptSpecifications { get; set; }
    public DbSet<SubProjectType> SubProjectTypes { get; set; }
    public DbSet<SubProjectTypeFilmSpecification> SubProjectTypeFilmSpecifications { get; set; }
    public DbSet<SubProjectTypeMusicSpecification> SubProjectTypeMusicSpecifications { get; set; }
    public DbSet<SubProjectTypeScriptSpecificaion> SubProjectTypeScriptSpecificaions { get; set; }
    public DbSet<SubProjectTypeVRXrSpecification> SubProjectTypeVRXrSpecifications { get; set; }
    public DbSet<XrVrSpecification> XrVrSpecifications { get; set; }
    public DbSet<ProjectImage> ProjectImages { get; set; }

    #endregion

    #region Venue

    public DbSet<Venue> Venues { get; set; }
    public DbSet<ShowHall> ShowHalls { get; set; }
    public DbSet<ShowTime> ShowTimes { get; set; }

    #endregion

    #region Tickets

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<SoldTicket> SealedTickets { get; set; }

    #endregion

    #region content

    public DbSet<New> News { get; set; }
    public DbSet<StaticPageAndFAQ> StaticPages { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Comment> Comments { get; set; }

    #endregion

    #region Advertise

    public DbSet<AttachFile> AttachFiles { get; set; }
    public DbSet<AdvertiseBanner> AdvertiseBanners { get; set; }
    public DbSet<AdvertiseRequest> AdvertiseRequests { get; set; }

    #endregion

    #region Notification

    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<ProductImage> ProductImages { get; set; }

    #endregion

    #region Seo

    public DbSet<MetaTag> MetaTags { get; set; }

    #endregion
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    break;
            }
        }

        if (_currentUserService.UserId == null)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        else
        {
            return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // The production database and the restored local database keep the
        // application tables in this schema. Relying on the SQL login's
        // default schema made local Windows-authenticated requests resolve
        // tables such as ArtCategories against dbo and fail.
        builder.HasDefaultSchema("hisubmi1_user");

        foreach (var property in builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        base.OnModelCreating(builder);
        builder.Entity<ChatHistory<BlazorHeroUser>>(entity =>
        {
            entity.ToTable("ChatHistory");
            entity.HasOne(d => d.FromUser)
                .WithMany(p => p.ChatHistoryFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.ToUser)
                .WithMany(p => p.ChatHistoryToUsers)
                .HasForeignKey(d => d.ToUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
        builder.Entity<BlazorHeroUser>(entity =>
        {
            entity.ToTable(name: "Users", "Identity");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        builder.Entity<BlazorHeroRole>(entity => { entity.ToTable(name: "Roles", "Identity"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles", "Identity"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims", "Identity"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins", "Identity"); });
        builder.Entity<BlazorHeroRoleClaim>(entity =>
        {
            entity.ToTable(name: "RoleClaims", "Identity");
            entity.HasOne(d => d.Role)
                .WithMany(p => p.RoleClaims)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens", "Identity"); });
        builder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(BlazorHeroContext))!);
    }
}
