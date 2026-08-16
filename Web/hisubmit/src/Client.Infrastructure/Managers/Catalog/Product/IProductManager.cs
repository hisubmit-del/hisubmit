using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Catalog;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.Product
{
    public interface IProductManager : ITransientManager
    {
        Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

        Task<IResult<string>> GetProductImageAsync(int id,int festivalId);

        Task<IResult<int>> SaveAsync(AddEditProductRequest request);

        Task<IResult<int>> DeleteAsync(int id,int festivalId);

        Task<IResult<string>> ExportToExcelAsync(int festivalId,string searchString = "");
        Task<IResult<AddEditProductRequest>> GetByIdAsync(GetProductByIdRequest request,int festivalId);
    }
}