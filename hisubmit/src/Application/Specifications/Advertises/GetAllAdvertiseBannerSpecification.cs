using System;
using HiSubmit.Application.Features.Advertises.Queries;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Advertise;

namespace HiSubmit.Application.Specifications.Advertises;

public class GetAllAdvertiseBannerSpecification : HeroSpecification<AdvertiseBanner>
{
    public GetAllAdvertiseBannerSpecification(GetAllAdvertiseBannerQuery query)
    {
        Criteria = (advertise) =>
            (string.IsNullOrWhiteSpace(query.SearchString) || advertise.Title.Contains(query.SearchString))
            && (query.IsOpen == null ||
                CheckOpenOrClose(query.IsOpen.Value, advertise.OpenDateTime, advertise.CloseDateTime));
    }

    private static bool CheckOpenOrClose(bool open, DateTime openTime, DateTime closeTime)
    {
        var now = DateTime.Now;
        if (open)
            return now >= openTime && now <= closeTime;

        return now < openTime || now > closeTime;
    }
}
