using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.DeleteFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Catalogs
{

    public class FestivalFocusEndPoint
    {
        private const string _route = "api/v1/festivalfocus/";
        public const string UppdateEventCategory = $"{_route}Update";
        public static string GetAllFestivalFocus(GetAllFestivalFocusQuery query)
        {
            var route = $"{_route}Getall?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string GetFestivalFocusDetaiil(GetFestivalFocusDeailQuery query)
        {
            var route = $"{_route}GetById?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string DeleteFestivalFocus(DeleteFestivalFocusCommand command)
        {
            var route = $"{_route}Delete?{QueryHelper.GetQueryString(command)}";
            return route;
        }
    }
}
