using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Specifications.Reviews;

public class FestivalReviewSpecification:HeroSpecification<Review>
{
    public FestivalReviewSpecification(int? festivalId)
    {
        Criteria = (review) => festivalId == null || review.FestivalId == festivalId.Value;
    }
}