using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using System;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;
using JudgingStatus = Hisubmit.Client.SharedModels.Enums.JudgingStatus;

namespace HiSubmit.Application.Specifications.Submits;

public class GetAllSubmitStatusSpecification : HeroSpecification<Submit>
{
    public GetAllSubmitStatusSpecification(SubmitStatus? submitStatus)
    {
        Criteria = submit => (submitStatus == null || submit.SubmitStatus == submitStatus);
    }
}


public class GetAllSubmitsFilterSpecification : HeroSpecification<Submit>
{
    public GetAllSubmitsFilterSpecification(GetAllSubmitsQuery query)
    {
        AddInclude(submit => submit.Project);
        AddInclude(submit => submit.Festival);
        Criteria = (submit) =>(submit.SubmitStatus!=SubmitStatus.DontPaid && submit.SubmitStatus!=SubmitStatus.Default)&& (string.IsNullOrEmpty(query.SearchString) || submit.Project.Title.Contains(query.SearchString) 
                                                                         || submit.Festival.Name.Contains(query.FestivalName))
                               && (string.IsNullOrEmpty(query.ProjectTitle) || submit.Project.Title.Contains(query.ProjectTitle))
                               && (string.IsNullOrEmpty(query.FestivalName) || submit.Festival.Name.Contains(query.FestivalName))
                               &&(string.IsNullOrWhiteSpace(query.UserId) || submit.Project.UserId==query.UserId)
                               &&(string.IsNullOrWhiteSpace(query.TrackingCode) || submit.TrackingCode==query.TrackingCode)
                               &&(query.SubmitDateFrom==null || query.SubmitDateFrom<=submit.SubmitDate)
                               &&(query.SubmitDateTo==null || query.SubmitDateTo>=submit.SubmitDate)
                               &&(query.SubmitStatus ==null || submit.SubmitStatus ==(SubmitStatus)query.SubmitStatus.Value)
                               &&(query.JudgingStatus ==null || submit.JudgingStatus ==(Domain.Enums.JudgingStatus)query.JudgingStatus.Value)
                               &&(query.FestivalId ==null || submit.FestivalId==query.FestivalId)
                               &&(query.ProjectId ==null || submit.ProjectId == query.ProjectId);
    }
}

