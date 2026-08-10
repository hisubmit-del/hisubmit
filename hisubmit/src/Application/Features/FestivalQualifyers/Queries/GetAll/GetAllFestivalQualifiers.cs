using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Domain.Entities.Festivals;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.FestivalQualifyers.Queries.GetAll;

public class GetAllFestivalQualifiersQuery 
    : IRequest<Result<List<GetAllFestivalQualifiersResponse>>>
{
    public int? FestivalId { get; set; }
}

internal class GetAllFestivalQualifiersQueryHandler(
    IMapper mapper,
    IStringLocalizer<GetAllFestivalQualifiersQueryHandler> localizer,
    IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetAllFestivalQualifiersQuery, Result<List<GetAllFestivalQualifiersResponse>>>
{
    private readonly IStringLocalizer<GetAllFestivalQualifiersQueryHandler> _localizer = localizer;

    public async Task<Result<List<GetAllFestivalQualifiersResponse>>>
        Handle(GetAllFestivalQualifiersQuery request, CancellationToken cancellationToken)
    {
        if (request.FestivalId == null)
        {
            var qualifiers = await unitOfWork.Repository<FestivalQualifying>()
                .GetAllAsync();
            var mappedQualifiers = mapper.Map<List<GetAllFestivalQualifiersResponse>>(qualifiers);
            return await Result<List<GetAllFestivalQualifiersResponse>>.SuccessAsync(mappedQualifiers);
        }
        else
        {
            var qualifiers = await unitOfWork.Repository<FestivalFestivalQualifying>()
                .Entities.Where(p => p.FestivalId == request.FestivalId)
                .Select(p => p.FestivalQualifying)
                .ProjectTo<GetAllFestivalQualifiersResponse>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            return await Result<List<GetAllFestivalQualifiersResponse>>.SuccessAsync(qualifiers);
        }
    }
}