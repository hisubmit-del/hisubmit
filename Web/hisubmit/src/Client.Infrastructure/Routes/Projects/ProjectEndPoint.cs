using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Projects
{
    public static  class ProjectEndPoint
    {
        private const string _route = "api/v1/project/";
        public static string GetAll(GetAllProjectRequest request)
        {
            var route = $"{_route}getAll?{QueryHelper.GetQueryString(request)}";
            return route;
        }

        public static string GetDetail(GetProjectDetailQuery query)
        {
            var route = $"{_route}detail?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string UpdateDetail()
        {
            var route = $"{_route}updateDetail";
            return route;
        }
        public static string UpdateSubmmiiter()
        {
            var route = $"{_route}UpdateSubmitter";
            return route;
        }

        public static string UpdateCredit()
        {
            var route = $"{_route}UpdateCredit";
            return route;
        }


        public static string UpdateDistribuation()
        {
            var route = $"{_route}UpdateDistribuationInformation";
            return route;
        }
        public static string UpdateAward()
        {
            var route = $"{_route}UpdateAwards";
            return route;
        }
        public static string UpdateScreenAward()
        {
            var route = $"{_route}UpdateScreenAwards";
            return route;
        }

        public static string AllCredits(GetAllProjectCreditQuery query)
        {
            var route = $"{_route}AllCredits?{QueryHelper.GetQueryString(query)}";
            return route;
        }

        public static string AllDistribuation(GetAllDistribuationInformationQuery query)
        {
            var route = $"{_route}DetailDistribuationInformation?{QueryHelper.GetQueryString(query)}";
            return route;
        }


        public static string AllAwards(GetAwardDetailRequest request)
        {
            var route = $"{_route}DetailAwards?{QueryHelper.GetQueryString(request)}";
            return route;
        }
        public static string AllScreenAwards(GetScreenAwardRequest request)
        {
            var route = $"{_route}DetailScreenAwards?{QueryHelper.GetQueryString(request)}";
            return route;
        }

        public static string UpdateFileURL()
        {
            var route = $"{_route}UpdateFileURL";
            return route;
        }
        public static string UploadFile()
        {
            var route = $"{_route}UploadFile";
            return route;
        }
    }
}
