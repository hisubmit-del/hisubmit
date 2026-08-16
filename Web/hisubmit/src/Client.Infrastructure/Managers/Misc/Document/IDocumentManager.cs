using Hisubmit.Client.SharedModels.Features.Documents.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Documents.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Documents.Queries.GetById;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Documents;

namespace HiSubmit.Client.Infrastructure.Managers.Misc.Document
{
    public interface IDocumentManager : ITransientManager
    {
        Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

        Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}