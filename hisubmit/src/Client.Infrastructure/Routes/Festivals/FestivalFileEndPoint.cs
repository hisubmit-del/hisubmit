using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail;

namespace HiSubmit.Client.Infrastructure.Routes.Festivals
{
    public static class FestivalFileEndPoint
    {
        private const string _route = "api/v1/FestivalFile/";
        private static string GetRoute(int festivalId)
        {
            return string.Format($"{_route}{festivalId}/");
        }

        public static string GetAll(GetAllFestivalFileQuery query,int festivalId)
        {
            var route = $"{GetRoute(festivalId)}getAll?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string GetDetail(GetFestivalFileDetailQuery query,int festivalId)
        {
            var route = $"{GetRoute(festivalId)}detail?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string Update(int festivalid)
        {
            var route = $"{GetRoute(festivalid)}update";
            return route;
        }
        public static string Delete(DeleteFestivalFileCommand command,int festivalId)
        {
            var route = $"{GetRoute(festivalId)}delete?{QueryHelper.GetQueryString(command)}";
            return route;
        }
    }
}
