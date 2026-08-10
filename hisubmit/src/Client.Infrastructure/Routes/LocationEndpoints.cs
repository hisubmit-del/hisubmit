using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes
{
    public static class LocationEndpoints
    {
        private static string _route = "api/v1/location/";
        public static string GetAllCountries(GetAllCountryQuery query)
        {
            return $"{_route}GetAllCountries{QueryHelper.GetQueryString(query)}";
        }
    }
}
