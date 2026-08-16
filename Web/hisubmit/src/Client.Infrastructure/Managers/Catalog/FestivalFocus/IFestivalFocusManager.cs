using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.DeleteFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.FestivalFocus
{
    public interface IFestivalFocusManager : ITransientManager
    {
        Task<IResult<List<GetAllFestivalFocusResponse>>> GetAllAsync(GetAllFestivalFocusQuery query);

        Task<IResult<int>> SaveAsync(AddEditFestivalFocusCommand request);

        Task<IResult<int>> DeleteAsync(DeleteFestivalFocusCommand command);
        Task<IResult<GetFestivalFocusDetailResponse>> GetById(GetFestivalFocusDeailQuery query);
    }
}
