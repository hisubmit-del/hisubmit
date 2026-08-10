using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Catalog;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.Export;

namespace HiSubmit.Application.Features.Brands.Queries.Export;

public class ExportBrandsQuery(string searchKey) 
    : ExportBrandsRequest(searchKey), IRequest<Result<string>>;

internal class ExportBrandsQueryHandler(
    IExcelService excelService,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<ExportBrandsQueryHandler> localizer)
    : IRequestHandler<ExportBrandsQuery, Result<string>>
{
    public async Task<Result<string>> Handle(ExportBrandsQuery request, CancellationToken cancellationToken)
    {
        var brandFilterSpec = new BrandFilterSpecification(request.SearchString);
        var brands = await unitOfWork.Repository<ArtCategory>().Entities
            .Specify(brandFilterSpec)
            .ToListAsync(cancellationToken);
        var data = await excelService.ExportAsync(brands, mappers: new Dictionary<string, Func<ArtCategory, object>>
        {
            { localizer["Id"], item => item.Id },
            { localizer["Name"], item => item.Name },
            { localizer["Description"], item => item.Description }
            //{ _localize["Tax"], item => item.Tax }
        }, sheetName: localizer["ArtCategory"]);

        return await Result<string>.SuccessAsync(data: data);
    }
}