using Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.Brand
{
    public interface IArtCategoryManager : ITransientManager
    {
        Task<IResult<List<GetAllArtCategoryResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditArtCatgoryRequest request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}