using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll;
namespace HiSubmit.Application.Specifications.ProjectJudgings;

public sealed class GetAllProjectJudgingFilterSpecification : HeroSpecification<ProjectJudging>
{
    public GetAllProjectJudgingFilterSpecification(GetAllProjectJudgingQuery query)
    {
        AddInclude(p => p.Submit);

        Criteria = projectJudging =>
            (query.SubmitId == null || query.SubmitId == projectJudging.SubmitId) &&
            (string.IsNullOrWhiteSpace(query.UserId) || projectJudging.UserId == query.UserId) &&
            (query.FestivalId == null || query.FestivalId == projectJudging.Submit.FestivalId) &&
            (!query.GetCurrentUser || projectJudging.RefereeStatus == Domain.Enums.RefereeStatus.Default);
    }
}
