using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.MediaRights.Queries
{
    public class GetAllMediaRightQuery:IRequest<Result<List<GetAllMediaRightResponse>>>
    {
    }

    public class GetMediaRightQueryHandler : IRequestHandler<GetAllMediaRightQuery, Result<List<GetAllMediaRightResponse>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetMediaRightQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<GetAllMediaRightResponse>>> Handle(GetAllMediaRightQuery request, CancellationToken cancellationToken)
        {
            var rights = await _unitOfWork.Repository<MediaRight>()
                .Entities
                .ProjectTo<GetAllMediaRightResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return await Result<List<GetAllMediaRightResponse>>.SuccessAsync(rights);
        }
    }

    public class GetAllMediaRightResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
