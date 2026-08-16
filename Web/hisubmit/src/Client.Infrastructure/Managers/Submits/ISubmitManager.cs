using Hisubmit.Client.SharedModels.Features.Submits.Commands;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.Submits;

public interface ISubmitManager : ITransientManager
{
    Task<IResult<int>> SubmitToFestival(AddSubmitCommand command);
    Task<PaginatedResult<GetAllSubmitsResponse>> GetAll(GetAllSubmitsRequest request);
    Task<IResult> FinalResult(AddEditFinalJudgingCommand command);
    Task<IResult> WithDraw(WithDrawProjectCommand command);
    Task<IResult> Review(AddReviewCommand command);
    Task<PaginatedResult<GetAllReviewResponse>> GetAllReview(GetAllReviewQuery query);
}