using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalFiles
{
    public interface IFestivalFileManager:ITransientManager
    {
        Task<IResult<List<GetAllFestivalFileResponse>>> GetAllAsync(GetAllFestivalFileQuery query,int festivalId);
        Task<IResult<GetFestivalFileDetailResponse>> GetDetailAsync(GetFestivalFileDetailQuery query,int FestivalId);
        Task<IResult<int>> UpdateAsync(AddEditFestivalFileCommand commmand, int festivalId);
        Task<IResult<int>> DeleteAsync(DeleteFestivalFileCommand command,int festivalId);
    }
}
