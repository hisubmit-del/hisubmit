using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Projects
{
    public static class ProjectSpecification
    {
        private const string _route = "api/v1/projectSpecification/";
        public static string GetFilmSpecification(GetFilmSpecificationDetailRequest request)
        {
            var route = $"{_route}FilmSpecificationDetail?{QueryHelper.GetQueryString(request)}";
            return route;
        }

        public static string UpdateFilmSpecification()
        {
            var route = $"{_route}UpdateFilmSpecification";
            return route;
        }

        public static string GetMusicSpecification(GetMusicSpecificationDetailQuery query)
        {
            var route = $"{_route}MusicSpecificationDetail?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string UpdateMusicSpecification()
        {
            var route = $"{_route}UpdateMusicSpecification";
            return route;
        }

        public static string GetScriptSpecification(GetScriptSpecificationDetailQuery query)
        {
            var route = $"{_route}ScriptSpecificationDetail?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string UpdateScriptSpecification()
        {
            var route = $"{_route}UpdateScriptSpecification";
            return route;
        }

        public static string GetPhotographySpecification(GetPhotographySpecificationDetailQuery query)
        {
            var route = $"{_route}PhotographySpecificationDetail?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string UpdatePhotographySpecification()
        {
            var route = $"{_route}UpdatePhotographySpecification";
            return route;
        }

        public static string GetVrXrSpecification(GetVrXrSpecificationDetailQuery query)
        {
            var route = $"{_route}VrXrSpecificationDetail?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string UpdateVrXrSpecification()
        {
            var route = $"{_route}UpdateVrXrSpecification";
            return route;
        }
    }
}
