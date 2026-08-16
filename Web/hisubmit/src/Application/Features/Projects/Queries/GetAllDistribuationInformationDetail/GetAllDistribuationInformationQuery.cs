using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Features.DistributionInformations.Commands;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Projects;
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

namespace HiSubmit.Application.Features.Projects.Queries.GetAllDistribuationInformationDetail;

public class GetAllDistribuationInformationQuery:IRequest<Result<List<AddEditDistributionInformationRequest>>>
{
    public int ProjectId { get; set; }
}

internal class GetAllDistribuationInformationQueryhandler : IRequestHandler<GetAllDistribuationInformationQuery, Result<List<AddEditDistributionInformationRequest>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllDistribuationInformationQueryhandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<AddEditDistributionInformationRequest>>> Handle(GetAllDistribuationInformationQuery request, CancellationToken cancellationToken)
    {
        var spec = new DistribuationInformationFilterSpecification(request.ProjectId);
        var awards = await _unitOfWork.Repository<DistributionInformation>()
            .Entities
            .Include(p=>p.Items).ThenInclude(p=>p.MediaRightDistributionInformation)
            .Specify(spec)
            .ProjectTo<AddEditDistributionInformationRequest>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return await Result<List<AddEditDistributionInformationRequest>>.SuccessAsync(awards);

    }
}