using HiSubmit.Domain.Entities.Submitter;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Events.Submits;

public class ProjectSubmitedEvent : INotification
{
    public string UserId { get; set; }
    public int SubmitId { get; set; }
    public double Price { get; set; }
    public string Title { get; set; }
    public  string ImageUrl { get; set; }
    public string ProjectName { get; set; }
    public string FestivalName { get; set; }
    public  FeeStatus FeeStatus { get; set; }
}

