using Hisubmit.Client.SharedModels.Features.MediaRights.Queries;

namespace HiSubmit.Client.Infrastructure.Routes.Catalogs
{
    public static class MediaRightEndPoint
    {
        private const string _route = "api/v1/MediaRight/";
        public static string GetAll(GetAllMediaRightQuery query)
        {
            var route = $"{_route}getall?{QueryHelper.GetQueryString(query)}";
            return route;
        }
    }
}
