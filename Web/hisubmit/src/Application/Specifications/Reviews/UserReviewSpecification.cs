using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Specifications.Reviews;

public class UserReviewSpecification:HeroSpecification<Review>
{
    public UserReviewSpecification(string userId)
    {
        Criteria = (review) => string.IsNullOrWhiteSpace(userId) || review.UserId == userId;
    }
}