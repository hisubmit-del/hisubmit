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

namespace HiSubmit.Application.Features.MonetaryUnits.Queries
{
    public class GetAllMonetaryUnitQuery:IRequest<Result<List<GetAllMonetaryUnitRespnse>>>
    {

    }

    public class GetAllMonetaryUnitQueryhandler : IRequestHandler<GetAllMonetaryUnitQuery, Result<List<GetAllMonetaryUnitRespnse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllMonetaryUnitQueryhandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllMonetaryUnitRespnse>>> Handle(GetAllMonetaryUnitQuery request, CancellationToken cancellationToken)
        {
            var units =await  _unitOfWork.Repository<MonetaryUnit>()
                .Entities.AsQueryable().ProjectTo<GetAllMonetaryUnitRespnse>(_mapper.ConfigurationProvider).ToListAsync();
            return await Result<List<GetAllMonetaryUnitRespnse>>.SuccessAsync(units);
        }
    }

    public class GetAllMonetaryUnitRespnse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
