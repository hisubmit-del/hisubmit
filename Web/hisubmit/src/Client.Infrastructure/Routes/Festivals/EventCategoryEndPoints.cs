using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetEventCateoryById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Festivals
{
    public class EventCategoryEndPoints
    {
        private const string _route = "api/v1/EventCategory/";

        public const string UppdateEventCategory=$"{_route}UpdateCategory";
        public static string GetAllEventCategory(GetAllEventCategoryQuery query)
        {
            var route = $"{_route}AllCategory?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string GetEventCategoryById(GetEventCategoryByIdQuery query)
        {
            var route = $"{_route}GetById?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string DeleteEventCategory(DeleteEventCategoryCommand command)
        {
            var route = $"{_route}Delete?{QueryHelper.GetQueryString(command)}";
            return route;
        }
    }
}
