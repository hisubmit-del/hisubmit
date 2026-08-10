using HiSubmit.Application.Features.Festivals.Queries.GetAllOrginizer;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Specifications.Festivals;

public class GetAllFestivalOrganizerSpecification:HeroSpecification<EventOrginizer>
{
    public GetAllFestivalOrganizerSpecification(GetAllOrganizerQuery query)
    {
        Criteria = (eventOrganizer) => eventOrganizer.FestivalId == query.FestivalId;
    }
}