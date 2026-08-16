using Hisubmit.Client.SharedModels.Features.MonetaryUnits.Queries;

namespace HiSubmit.Client.Infrastructure.Routes.Catalogs
{
    public static class MonetaryUnitsEndPoints
    {
        private const string _route = "api/v1/monetaryunits/";
        public static string GetAll(GetAllMonetaryUnitQuery query)
        {
            var route = $"{_route}getall?{QueryHelper.GetQueryString(query)}";
            return route;
        }
    }
}
