using AutoMapper;
using HiSubmit.Application.Features.Reviews.Commands;
using HiSubmit.Application.Features.Reviews.Queries;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Mappings;

public class ReviewProfile:Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, AddReviewCommand>().ReverseMap();
        CreateMap<Review, GetAllReviewResponse>().ReverseMap();
    }
}