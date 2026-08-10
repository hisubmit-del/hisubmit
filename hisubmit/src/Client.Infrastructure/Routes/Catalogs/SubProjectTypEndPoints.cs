using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;

namespace HiSubmit.Client.Infrastructure.Routes.Catalogs
{
    public static class SubProjectTypEndPoints
    {
        private const string _route = "api/v1/subprojecttype/";
        public static string GetAll(GetAllSubProjectTypeQuery query)
        {
            var route = $"{_route}getall?{QueryHelper.GetQueryString(query)}";
            return route;
        }
    }
}
