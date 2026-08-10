using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Infrastructure.Contexts;
using HiSubmit.Infrastructure.Helpers;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Infrastructure.Validators;
using HiSubmit.Shared.Constants.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Domain.Entities.Payments;

namespace HiSubmit.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localize;
        private readonly BlazorHeroContext _db;
        private readonly UserManager<BlazorHeroUser> _userManager;
        private readonly RoleManager<BlazorHeroRole> _roleManager;

        public DatabaseSeeder(
            UserManager<BlazorHeroUser> userManager,
            RoleManager<BlazorHeroRole> roleManager,
            BlazorHeroContext db,
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localize)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleManager.RoleValidators.Clear();
            _roleManager.RoleValidators.Add(new FestivalRoleValidator());
            _db = db;
            _logger = logger;
            _localize = localize;
        }

        public void Initialize()
        {
            AddAdminstratorRole();
            AddFestivalRole();
            AddAdministrator();
            AddBasicUser();
            AddFestivalRole();
            AddFestivalQualifying();
            AddSubProjectType();
            AddMonetaryUnits();
            AddMediaRights();
            AddSiteCommission();
            _db.SaveChanges();
            AddRefreeRoleToFestival();
            _db.SaveChanges();
        }

        private void AddFestivalQualifying()
        {
            if (!_db.FestivalQualifyings.Any())
            {
                _db.FestivalQualifyings.AddRange(new List<FestivalQualifying>()
                {
                    new()
                    {
                        Name = "Canadian screen award",
                        LogoName = @"img\FestivalQualifying\Canadian.jpg"
                    },
                    new()
                    {
                        Name = "Oscar Academy Awards",
                        LogoName = @"img\FestivalQualifying\academy.png"
                    },
                    new()
                    {
                        Name = "BAFTA",
                        LogoName = @"img\FestivalQualifying\bafta.png"
                    },
                    new()
                    {
                        Name = "BAFTA Cymru",
                        LogoName = @"img\FestivalQualifying\baftaCymru.png"
                    },
                    new()
                    {
                        Name = "FIAPF",
                        LogoName = @"img\FestivalQualifying\fiapf.png"
                    },
                    new()
                    {
                        Name = "GOYA",
                        LogoName = @"img\FestivalQualifying\goya.png"
                    },
                    new()
                    {
                        Name = "Melies D'or",
                        LogoName = @"img\FestivalQualifying\melies.png"
                    },
                    new()
                    {
                        Name = "Cartoon d’Or",
                        LogoName = @"img\FestivalQualifying\cartons.jpg"
                    },
                    new()
                    {
                        Name = "EAA (European animation awards)",
                        LogoName = @"img\FestivalQualifying\eaa_black.png"
                    }
                });
            }
        }


        private void AddMediaRights()
        {
            if (!_db.MediaRights.Any())
            {
                _db.MediaRights.AddRange(new List<MediaRight>()
                {
                    new MediaRight() { Name = "All Rights" },
                    new MediaRight() { Name = "Video on Demond" },
                    new MediaRight() { Name = "Hotel" },
                    new MediaRight() { Name = "Ship" },
                    new MediaRight() { Name = "Video/Disc" },
                    new MediaRight() { Name = "Paid Tv" },
                    new MediaRight() { Name = "Internet" },
                    new MediaRight() { Name = "Pay Per View" },
                    new MediaRight() { Name = "AirLine" },
                    new MediaRight() { Name = "Theatrical" },
                    new MediaRight() { Name = "Free Tv" },
                    new MediaRight() { Name = "Console/Handheld" },
                });
            }
        }

        private void AddMonetaryUnits()
        {
            if (!_db.MonetaryUnits.Any())
            {
                _db.MonetaryUnits.AddRange(new List<MonetaryUnit>()
                {
                    new MonetaryUnit() { Name = "USD" },
                    new MonetaryUnit() { Name = "EUR" },
                    new MonetaryUnit() { Name = "AUD" },
                    new MonetaryUnit() { Name = "GBP" },
                    new MonetaryUnit() { Name = "JPY" },
                    new MonetaryUnit() { Name = "CHF" },
                    new MonetaryUnit() { Name = "NZD" },
                    new MonetaryUnit() { Name = "CNY" },
                    new MonetaryUnit() { Name = "MXN" }
                });
            }
        }

        private void AddSubProjectType()
        {
            if (!_db.SubProjectTypes.Any())
            {
                _db.SubProjectTypes.AddRange(new List<SubProjectType>()
                {
                    new SubProjectType() { Name = "Animation", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Documentory", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Experimental", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Feature", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Music Video", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Short", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Student", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Television", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Web/ New media", ProjectType = ProjectType.Film },
                    new SubProjectType() { Name = "Other", ProjectType = ProjectType.Film },

                    new SubProjectType() { Name = "Student", ProjectType = ProjectType.Music },
                    new SubProjectType() { Name = "Song", ProjectType = ProjectType.Music },
                    new SubProjectType() { Name = "Film Score", ProjectType = ProjectType.Music },
                    new SubProjectType() { Name = "Lyris Only", ProjectType = ProjectType.Music },
                    new SubProjectType() { Name = "Other", ProjectType = ProjectType.Music },

                    new SubProjectType() { Name = "Student", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Screen play", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Short script", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "State play", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Television Script", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Treatment", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Literary Fiction", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Novel", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Short story", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Flash fiction", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "TV pilot", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Series", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Poem", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Critical", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Prose", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Article", ProjectType = ProjectType.Script_ScreenWriting },
                    new SubProjectType() { Name = "Other", ProjectType = ProjectType.Script_ScreenWriting },


                    new SubProjectType() { Name = "Student", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Virual Reality", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Performance", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Installation", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Game", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "InterActive Film", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "360 Video", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Augmented Reality", ProjectType = ProjectType.VR_XR },
                    new SubProjectType() { Name = "Other", ProjectType = ProjectType.VR_XR },


                    new SubProjectType() { Name = "Nature", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Landscape", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Wildlife", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Marco", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Micro", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Astrophotography", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "People", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Scintific", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Portrait", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Dicumentary", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Sport", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Fashion", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Travel", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Pet", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Commerical", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Man made", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Product", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Food", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Architecture", ProjectType = ProjectType.Photography },
                    new SubProjectType() { Name = "Other", ProjectType = ProjectType.Photography },
                });
            }
        }

        private void AddSiteCommission()
        {
            Task.Run(async () =>
            {
                if (!_db.SiteCommissions.Any())
                {
                    await _db.SiteCommissions.AddAsync(
                        new SiteCommission()
                        {
                            MaximumServiceFee = 10,
                            MinimumServiceFee = 1,
                            ProductSalesCommission = 10,
                            SpecialFestivalCommission = 2,
                            SubmitServiceFee = 5,
                            TicketSalesCommission = 7,
                            UsualFestivalCommission = 4,
                            MonthlySpecialUserFee = 9.99,
                            ThreeMonthlySpecialUserFee = 24.99,
                            YearlySpecialUserFee = 90.99
                        });
                }
            }).GetAwaiter().GetResult();
        }

        private void AddFestivalRole()
        {
            Task.Run(async () =>
            {
                var festivalRole =
                    new BlazorHeroRole(RoleConstants.FestivalRole, null, _localize["Manger of FestivalId"]);
                var festivalRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.FestivalRole);
                if (festivalRoleInDb == null)
                {
                    await _roleManager.CreateAsync(festivalRole);
                    festivalRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.FestivalRole);
                }

                var dbPermissions = await _roleManager.GetClaimsAsync(festivalRoleInDb);

                foreach (var permission in Permissions.GetRegisteredFestivalPermissions())
                {
                    if (dbPermissions.All(p => p.Value != permission))
                    {
                        await _roleManager.AddPermissionClaim(festivalRoleInDb, permission);
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private void AddAdminstratorRole()
        {
            Task.Run(async () =>
            {
                var adminRole = new BlazorHeroRole(RoleConstants.AdministratorRole,
                    _localize["Administrator role with full permissions"]);
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                }

                var dbPermissions = await _roleManager.GetClaimsAsync(adminRole);
                foreach (var permission in Permissions.GetRegisteredAdminPermissions())
                {
                    if (dbPermissions.All(p => p.Value != permission))
                    {
                        await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if User Exists
                var superUser = new BlazorHeroUser
                {
                    FirstName = "Amir",
                    LastName = "Mohammadi",
                    Email = "Amir@Mohammadi.com",
                    UserName = "Amir",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localize["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var basicRole = new BlazorHeroRole(RoleConstants.ArtistRole,
                    _localize["Basic role with default permissions"]);
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.ArtistRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                    _logger.LogInformation(_localize["Seeded Basic Role."]);
                }

                //Check if User Exists
                var basicUser = new BlazorHeroUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@blazorhero.com",
                    UserName = "johndoe",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(basicUser, RoleConstants.ArtistRole);
                    _logger.LogInformation(_localize["Seeded User with Basic Role."]);
                }
            }).GetAwaiter().GetResult();
        }


        private void AddRefreeRoleToFestival()
        {
            Task.Run(async () =>
            {
                var festivalRoleIds = _db.Roles
                    .Where(p => p.FestivalId != null && p.Name == RoleConstants.Referee)
                    .Select(p => p.FestivalId.Value)
                    .ToList();

                var festivalsId = _db.Festivals.Select(p => p.Id).ToList();

                var festivalHavNotRefereeRole = festivalsId
                    .Where(festivalId => !festivalRoleIds.Any(id => id == festivalId))
                    .ToList();

                //await _roleManager.CreateAsync(new BlazorHeroRole(RoleConstants.Referee + festivalhavNotRefreeRole[0], festivalhavNotRefreeRole[0], null)); 
                foreach (var festivalId in festivalHavNotRefereeRole)
                {
                    var result = await _roleManager.CreateAsync(new BlazorHeroRole()
                    {
                        Name = $"{RoleConstants.Referee}",
                        FestivalId = festivalId
                    });
                    if (!result.Succeeded)
                    {
                        Console.WriteLine(result.Errors.First().Description);
                    }
                    //  await _roleManager.CreateAsync(new BlazorHeroRole(RoleConstants.Referee+ festivalId, festivalId, null));
                }
            }).GetAwaiter().GetResult();
        }
    }
}
