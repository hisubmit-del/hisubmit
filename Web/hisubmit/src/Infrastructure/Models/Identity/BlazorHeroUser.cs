using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Infrastructure.Models.Identity;

public class BlazorHeroUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
{
    public string VerificationCode { get; set; }
    public string FirstName { get; set; }

    public string LastName { get; set; }
    public string CreatedBy { get; set; }


    [Column(TypeName = "text")]
    public string ProfilePictureDataUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }
    public bool IsActive { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryFromUsers { get; set; }
    public virtual ICollection<ChatHistory<BlazorHeroUser>> ChatHistoryToUsers { get; set; }
    public virtual ICollection<FestivalSubUser> FestivalSubUsers { get; set; }
 //   public List<FestivalId> Festivals { get; set; }
    


    public  List<FestivalMaster> FestivalMasters { get; set; }
    public List<ProjectJudging> ProjectJudgings { get; set; }
    public  List<SoldTicket> SealedTickets { get; set; }



    public  FeeStatus FeeStatus { get; set; }
    public  DateTime? FeeStatusExpirationDate { get; set; }
    public BlazorHeroUser()
    {
        ChatHistoryFromUsers = new HashSet<ChatHistory<BlazorHeroUser>>();
        ChatHistoryToUsers = new HashSet<ChatHistory<BlazorHeroUser>>();            
    }
}

