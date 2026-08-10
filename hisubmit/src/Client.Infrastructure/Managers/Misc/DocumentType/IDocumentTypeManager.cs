using Hisubmit.Client.SharedModels.Features.DocumentTypes.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.DocumentTypes.Queries.GetAll;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Misc.DocumentType
{
    public interface IDocumentTypeManager : ITransientManager
    {
        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);

        Task<IResult<string>> ExportToExcelAsync(string searchString = "");
    }
}