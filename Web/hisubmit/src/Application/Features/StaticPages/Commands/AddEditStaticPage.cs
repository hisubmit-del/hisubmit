using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using ContentType = Hisubmit.Client.SharedModels.Features.StaticPages.Commands.ContentType;

namespace HiSubmit.Application.Features.StaticPages.Commands;

public class AddEditStaticPageCommand : AddEditStaticPageRequest, IRequest<IResult>
{
}

public class AddEditStaticPageCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddEditStaticPageCommandHandler> localizer)
    : IRequestHandler<AddEditStaticPageCommand, IResult>
{
    public async Task<IResult> Handle(AddEditStaticPageCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            var n = mapper.Map<StaticPageAndFAQ>(request);
            var res = await unitOfWork.Repository<StaticPageAndFAQ>().AddAsync(n);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.Type != ContentType.Faq)
            {
                var _mappedSeoTag = mapper.Map<MetaTag>(request.SeoTag);
                _mappedSeoTag.Type = PageType.StaticPage;
                _mappedSeoTag.PageId = res.Id.ToString();
                _mappedSeoTag.PageTitle = res.Title;
                unitOfWork.Repository<MetaTag>().AddAsync(_mappedSeoTag);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(localizer["Static Page Added"]);
        }
        else
        {
            var newDb = await unitOfWork.Repository<StaticPageAndFAQ>().GetByIdAsync(request.Id);
            if (newDb == null) return await Result.FailAsync(localizer["Static page not found"]);
            var updatedNew = mapper.Map(request, newDb);
            await unitOfWork.Repository<StaticPageAndFAQ>().UpdateAsync(updatedNew);

            var dbSeoTags = await unitOfWork.Repository<MetaTag>()
                .Entities.
                Where(p => p.PageId == newDb.Id.ToString() && p.Type == PageType.StaticPage)
                .FirstOrDefaultAsync(cancellationToken);

            if (request.Type != ContentType.Faq)
            {
                if (dbSeoTags != null)
                {
                    var mappedUpdateSeoTag = mapper.Map(request.SeoTag, dbSeoTags);
                    await unitOfWork.Repository<MetaTag>().UpdateAsync(mappedUpdateSeoTag);
                }
                else
                {
                    var _mappedSeoTag = mapper.Map<MetaTag>(request.SeoTag);

                    _mappedSeoTag.Type = PageType.StaticPage;
                    _mappedSeoTag.PageId = request.Id.ToString();
                    _mappedSeoTag.PageTitle = request.Title;

                    unitOfWork.Repository<MetaTag>().AddAsync(_mappedSeoTag);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(localizer["Static page Updated"]);
        }
    }
}