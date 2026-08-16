using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using System.Linq;

namespace HiSubmit.Application.Specifications.Submits
{
    public class GetAllFestivalSubmitsSpecification : HeroSpecification<Submit>
    {
        public GetAllFestivalSubmitsSpecification(int? festivalId)
        {
            Criteria = submit => (festivalId == null || submit.FestivalId == festivalId);
        }
    }


    public sealed class GetAllDeadLineEventCategoryFilter : HeroSpecification<DeadlineEventCategory>
    {
        public GetAllDeadLineEventCategoryFilter
            (int size,bool studentProject , ProjectType? projectType,int? countryId)
        {
            AddInclude(p => p.EventCategory);
            Criteria = deadlineEventCategory =>
            (
                deadlineEventCategory.EventCategory.ProjectType == null || 
                deadlineEventCategory.EventCategory.ProjectType.Value == projectType)
            && (deadlineEventCategory.EventCategory.StudentProject==false ||  studentProject)
            && (
            (deadlineEventCategory.EventCategory.RuntimeType == RuntimeType.Under && deadlineEventCategory.EventCategory.FirstRunTimeValue <= size  )||
            (deadlineEventCategory.EventCategory.RuntimeType == RuntimeType.Over && deadlineEventCategory.EventCategory.FirstRunTimeValue >= size) ||
            (deadlineEventCategory.EventCategory.RuntimeType == RuntimeType.Beetween && deadlineEventCategory.EventCategory.FirstRunTimeValue <= size && deadlineEventCategory.EventCategory.SecoundRunTimeValue >=size ) 
            || (deadlineEventCategory.EventCategory.RuntimeType == null)
            )
            && 
            (( deadlineEventCategory.EventCategory.EventCategoryCountries==null
               || ! deadlineEventCategory.EventCategory.EventCategoryCountries.Any() || countryId==null || countryId==0)
            || deadlineEventCategory.EventCategory.EventCategoryCountries.Any(country=>country.CountryId==countryId.Value))
            ;
        }
    }
}
