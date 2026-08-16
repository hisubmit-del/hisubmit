using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgiingFiiled;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgingButtons;
using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Festivals
{
    public static class JudgingEndpoint
    {
        private const string _route = "api/v1/Judging/";
        private static string GetRoute(int festivalId)
        {
            return string.Format($"{_route}{festivalId}/");
        }
        public static string AddEditFiled(int festivalId)
        {
            var route = $"{GetRoute(festivalId)}UpdateFiled";
            return route;
        }
        public static string AddEditButton(int festivalId)
        {
            var route = $"{GetRoute(festivalId)}UpdateButton";
            return route;
        }
        public static string Detail(GetJudgingDetailQuery query)
        {
            var route = $"{GetRoute(query.FestivalId)}Detail?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string DeleteFiled(DeleteJudgingFiledCommand commmand,int festivalId)
        {
            var route = $"{GetRoute(festivalId)}DeleteFiled?{QueryHelper.GetQueryString(commmand)}";
            return route;
        }
        public static string DeleteButton(DeleteJudgingButtonCommand commmand, int festivalId)
        {
            var route = $"{GetRoute(festivalId)}DeleteButton?{QueryHelper.GetQueryString(commmand)}";
            return route;
        }
    }
}
