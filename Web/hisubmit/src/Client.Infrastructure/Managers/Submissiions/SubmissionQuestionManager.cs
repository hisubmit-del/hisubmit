using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Commands.Delete;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetAll;
using Hisubmit.Client.SharedModels.Features.Submission.SubmissionQuestions.Query.GetDetail;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Submissiions
{
    public interface ISubmissionQuestionManager : ITransientManager
    {
        Task<IResult<List<GetAllSubmissionQuestionResponse>>> GetAllAsync(GetAllSubmissionQuestionQuery query);
        Task<IResult<GetSubmissionQuestionDetailResponse>> GetDetailAsync(GetSubmissionQuestionDetailQuery query);
        Task<IResult<int>> UpdateAsync(AddEditSubmissionQuestionCommand commmand,int festivalId);
        Task<IResult<int>> DeleteAsync(DeleteSubmissionQuestionCommand command);
    }
    public class SubmissionQuestionManager : ISubmissionQuestionManager
    {
        private readonly HttpClient _httpClient;
        public SubmissionQuestionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<int>> DeleteAsync(DeleteSubmissionQuestionCommand command)
        {
            var response = await _httpClient.DeleteAsync(SubmissionQuestionEndPoint.Delete(command));
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllSubmissionQuestionResponse>>> GetAllAsync(GetAllSubmissionQuestionQuery query)
        {
            var response = await _httpClient.GetAsync(SubmissionQuestionEndPoint.GetAll(query));
            return await response.ToResult<List<GetAllSubmissionQuestionResponse>>();
        }

        public async Task<IResult<GetSubmissionQuestionDetailResponse>> GetDetailAsync(GetSubmissionQuestionDetailQuery query)
        {
            var response = await _httpClient.GetAsync(SubmissionQuestionEndPoint.GetDetail(query));
            return await response.ToResult<GetSubmissionQuestionDetailResponse>();
        }

        public async Task<IResult<int>> UpdateAsync(AddEditSubmissionQuestionCommand command ,int festivalId)
        {
            var response = await _httpClient.PostAsJsonAsync(SubmissionQuestionEndPoint.Update(festivalId),command);
            return await response.ToResult<int>();
        }
    }
}
