using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Entities.Locations;

namespace HiSubmit.Domain.Entities.Festivals;

public class EventCategory : AuditableEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    [ForeignKey(nameof(Festival))]
    public int FestivalId { get; set; }
    public Festival Festival { get; set; }

    //RunTime
    public ProjectType? ProjectType { get; set; }
    public RuntimeType? RuntimeType { get; set; }
    public int FirstRunTimeValue { get; set; }
    public int? SecoundRunTimeValue { get; set; }

    public bool RequirePassword { get; set; }
    public string Password { get; set; }
    public bool StudentProject { get; set; }

    //Locations
    public LocationType? LocationType { get; set; }

    //public int? CountryId { get; set; }
    public string CityOrStateName { get; set; }
    public List<EventCategoryCountry> EventCategoryCountries { get; set; }
    public List<DeadlineEventCategory> DeadlineEventCategories { get; set; }
    public List<SubmissionQuestionEventCategory> SubmissionQuestionEventCategories { get; set; }

    //Submission
}

public class EventCategoryCountry : AuditableEntity
{
    public int Id { get; set; }
    public Country Country { get; set; }
    public EventCategory EventCategory { get; set; }
    public int CountryId { get; set; }
    public int EventCategoryId { get; set; }
}