using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Domain.Entities.Submitter;

public  class Submit:AuditableEntity<int>
{
    public int FestivalId { get; set; }
    public Festival Festival { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; }

    public List<SubmitDeadLineCategories> SubmitDeadlineEventCategories { get; set; }
    public List<SubmitAnswerQuestion> SubmitAnswerQuestions { get; set; }

    public DateTime SubmitDate { get; set; }
    public SubmitStatus SubmitStatus { get; set; }
    public JudgingStatus JudgingStatus { get; set; }
    public string Comment { get; set; }
        
    public string TrackingCode { get; set; }

    public Submit()
    {
        SubmitDeadlineEventCategories = new List<SubmitDeadLineCategories>();
        SubmitAnswerQuestions= new List<SubmitAnswerQuestion>();
    }

        
    //navigation property
    public  List<ProjectJudging> ProjectJudgings { get; set; }
}