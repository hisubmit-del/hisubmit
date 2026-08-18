using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Products.Queries.GetProductImage
{
    public class GetProductImageQuery : IRequest<Result<string>>
    {
        public int Id { get; set; }
        public int? FestivalId { get; set; }

        public GetProductImageQuery(int productId, int? festivalId = null)
        {
            Id = productId;
            FestivalId = festivalId;
        }
    }

    internal class GetProductImageQueryHandler : IRequestHandler<GetProductImageQuery, Result<string>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetProductImageQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(GetProductImageQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.Repository<Product>().Entities
                .Where(p => p.Id == request.Id &&
                            (request.FestivalId == null || p.FestivalId == request.FestivalId))
                .Select(a => a.ImageDataURL)
                .FirstOrDefaultAsync(cancellationToken);
            return await Result<string>.SuccessAsync(data: data);
        }
    }
}
