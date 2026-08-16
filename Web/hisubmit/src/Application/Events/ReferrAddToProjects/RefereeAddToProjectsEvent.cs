using System.Collections.Generic;
using MediatR;

namespace HiSubmit.Application.Events.RefeerrAddToProjects;

public class RefereeAddToProjectsEvent:INotification
{
   public  int ProjectJudgingId { get; set; }
   public string UserId { get; set; }
}
