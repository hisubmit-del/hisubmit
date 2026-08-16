using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;

namespace HiSubmit.Client.Infrastructure.Routes.Festivals
{
    public static class FestivalQualifiresEndPoint
    {
        private const string _route = "api/v1/FestivalQualifiers/";

        public static string GetAll(GetAllFestivalQualifiersQuery query)
        {
            var route = $"{_route}getall?{QueryHelper.GetQueryString(query)}";
            return route;
        }
    }
}
