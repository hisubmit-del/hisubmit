using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.Delete;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Routes.Festivals
{
    public class SubmissionQuestionEndPoint
    {
        private const string _route = "api/v1/SubmissionQuestion/";
        private static string GetRoute(int? festivalId)
        {
            var id = festivalId ?? 0;
            return string.Format($"{_route}{id}/");
        }

        public static string GetAll(GetAllSubmissionQuestionQuery query)
        {
            var route = $"{GetRoute(query.FestivalId )}getAll?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string GetDetail(GetSubmissionQuestionDetailQuery query)
        {
            var route = $"{GetRoute(query.FestivalId)}detail?{QueryHelper.GetQueryString(query)}";
            return route;
        }
        public static string Update(int festivalid)
        {
            var route = $"{GetRoute(festivalid)}update";
            return route;
        }
        public static string Delete(DeleteSubmissionQuestionCommand command)
        {
            var route = $"{GetRoute(command.FestivalId)}delete?{QueryHelper.GetQueryString(command)}";
            return route;
        }
    }
}
