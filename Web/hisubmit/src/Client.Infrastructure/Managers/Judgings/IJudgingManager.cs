using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgiingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgingButton;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgiingFiiled;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.DeleteJudgingButtons;
using Hisubmit.Client.SharedModels.Features.Judgings.Queries.Detail;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Judgings
{
    public interface IJudgingManager:ITransientManager
    {
        Task<IResult<GetJudgingDetailResponse>> GetDetail(GetJudgingDetailQuery query);
        Task<IResult<int>> AddButton(AddEditJudgingButtonCommand command,int festivalId);
        Task<IResult<int>> AddFiled(AddEditJudgingFiledCommand command, int festivalId);
        Task<IResult<int>> DeleteButton(DeleteJudgingButtonCommand command, int festivalId);
        Task<IResult<int>> DeleteFiled(DeleteJudgingFiledCommand command, int festivalId);
    }
}
