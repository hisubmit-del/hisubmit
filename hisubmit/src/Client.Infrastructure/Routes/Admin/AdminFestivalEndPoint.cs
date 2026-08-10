using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;

namespace HiSubmit.Client.Infrastructure.Routes.Admin
{
    public static class AdminFestivalEndPoint
    {
        private static readonly string _route = "api/v1/admin/adminFestival/";
        public static string GetAll( )
        {
            var route = $"{_route}getAll";
            return route;
        }

        public static string UpdateState()
        {
            var route = $"{_route}updateState";
            return route;
        }

        public static string UpdateFeeStatus()
        {
            var route = $"{_route}UpdateFeeStatus";
            return route;
        }

        public static string GetDetail(GetFestivalDetailByIdQuery query)
        {
            var route = $"{_route}detail?{QueryHelper.GetQueryString(query)}";
            return route;
        }
    }
}
