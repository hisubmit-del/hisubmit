using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Infrastructure.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly IRepositoryAsync<ArtCategory, int> _repository;

        public BrandRepository(IRepositoryAsync<ArtCategory, int> repository)
        {
            _repository = repository;
        }
    }
}