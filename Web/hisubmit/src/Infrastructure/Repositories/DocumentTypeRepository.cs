using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Misc;

namespace HiSubmit.Infrastructure.Repositories
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly IRepositoryAsync<DocumentType, int> _repository;

        public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository)
        {
            _repository = repository;
        }
    }
}