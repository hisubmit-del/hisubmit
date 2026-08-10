using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.News.Commands;

public class AddEditNewCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string BannerUrl { get; set; }

    public string Description { get; set; }

    // public bool IsEnable { get; set; }
    public string ShortDescription { get; set; }

    public string ImageALt { get; set; }
    public int? FestivalId { get; set; }
    public UploadRequest UploadRequest { get; set; } = new();
    public AddEditSeoTagRequest SeoTag { get; set; } = new();
}

public class AddEditNewCommandHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddEditNewCommandHandler> localizer,
    IUploadService uploadService) : IRequestHandler<AddEditNewCommand, IResult>
{
    public async Task<IResult> Handle(AddEditNewCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == 0)
        {
            var n = mapper.Map<New>(request);

            n.BannerUrl = uploadService.UploadAsync(request.UploadRequest);
            var rd = await unitOfWork.Repository<New>().AddAsync(n);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var _mappedSeoTag = mapper.Map<MetaTag>(request.SeoTag);

            _mappedSeoTag.Type = PageType.News;
            _mappedSeoTag.PageId = rd.Id.ToString();
            _mappedSeoTag.PageTitle = rd.Title;

            unitOfWork.Repository<MetaTag>().AddAsync(_mappedSeoTag);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(localizer["New Added"]);
        }
        else
        {
            var newDb = await unitOfWork.Repository<New>().GetByIdAsync(request.Id);
            if (newDb == null) return await Result.FailAsync(localizer["new not found"]);
            var updatedNew = mapper.Map(request, newDb);

            await unitOfWork.Repository<New>().UpdateAsync(updatedNew);
            var dbSeoTags = await unitOfWork.Repository<MetaTag>()
                .Entities.Where(p => p.PageId == newDb.Id.ToString() && p.Type == PageType.News)
                .FirstOrDefaultAsync(cancellationToken);
            if (dbSeoTags != null)
            {
                var mappedUpdateSeoTag = mapper.Map(request.SeoTag, dbSeoTags);
                await unitOfWork.Repository<MetaTag>().UpdateAsync(mappedUpdateSeoTag);
            }
            else
            {
                var _mappedSeoTag = mapper.Map<MetaTag>(request.SeoTag);
                _mappedSeoTag.Type = PageType.News;
                _mappedSeoTag.PageId = request.Id.ToString();
                _mappedSeoTag.PageTitle = request.Title;
                unitOfWork.Repository<MetaTag>().AddAsync(_mappedSeoTag);
            }
            UpdateBannerURl(request.BannerUrl, newDb.BannerUrl, request.UploadRequest);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(localizer["New Updated"]);
        }
    }


    private string UpdateBannerURl(string dbRewardLogoUrl, string clientRewardLogoUrl,
        UploadRequest uploadRequest)
    {
        var updatedRewardLogoUrl = dbRewardLogoUrl;
        if (string.IsNullOrWhiteSpace(clientRewardLogoUrl))
        {
            TryDeleteOldBannerFile(dbRewardLogoUrl);
            updatedRewardLogoUrl = string.Empty;
        }

        if (uploadRequest != null && uploadRequest.Data.Any())
        {
            TryDeleteOldBannerFile(dbRewardLogoUrl);
            updatedRewardLogoUrl = uploadService.UploadAsync(uploadRequest);
        }

        return updatedRewardLogoUrl;
    }

    private void TryDeleteOldBannerFile(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        }
    }
}