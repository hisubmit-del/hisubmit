using System.Collections.Generic;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Domain.Entities.SeoTags;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Application.Events.Products;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Entities.Festivals;
using PageType = HiSubmit.Domain.Entities.SeoTags.PageType;
using ProductType = HiSubmit.Domain.Enums.ProductType;

namespace HiSubmit.Application.Features.Products.Commands.AddEdit;

public class AddEditProductCommand : AddEditProductRequest, IRequest<Result<int>>;

internal class AddEditProductCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IUploadService uploadService,
    IMediator mediator,
    ICurrentUserService currentUserService,
    IStringLocalizer<AddEditProductCommandHandler> localize)
    : IRequestHandler<AddEditProductCommand, Result<int>>
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Result<int>> Handle(AddEditProductCommand command, CancellationToken cancellationToken)
    {
        var isAdmin =  _currentUserService.IsInRole(RoleConstants.AdministratorRole);
        var uploadRequest = command.UploadRequest;
        uploadRequest.UploadType = UploadType.Product;

        if (uploadRequest != null)
        {
            uploadRequest.FileName = uploadRequest.FileName +"."+ uploadRequest.Extension;
        }

        var festival= await unitOfWork.Repository<Festival>().GetByIdAsync(command.FestivalId);

        if (command.Id == 0)
        {
            var product = mapper.Map<Product>(command);
            if (uploadRequest != null)
                product.ImageDataURL = uploadService.UploadAsync(uploadRequest);


            product.ProductImages.Clear();

            foreach (var pm in command.ProductImages)
            {
                var url = uploadService.UploadAsync(pm.UploadRequest);
                product.ProductImages.Add(new ProductImage() { Url = url });
            }

            var f = await unitOfWork.Repository<Product>().AddAsync(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            var mappedSeoTag = mapper.Map<MetaTag>(command.SeoTag);

            mappedSeoTag.Type = PageType.Product;
            mappedSeoTag.PageId = f.Id.ToString();
            mappedSeoTag.PageTitle = f.Name;

            unitOfWork.Repository<MetaTag>().AddAsync(mappedSeoTag);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await mediator.Publish(new AddProductByFestivalEvent()
            {
                FestivalId = product.FestivalId,
                ProductId = product.Id,
                FestivalName = festival.Name
            }, cancellationToken);

            return await Result<int>.SuccessAsync(product.Id, localize["Product Saved"]);
        }
        else
        {
            var product = await unitOfWork.Repository<Product>().GetByIdAsync(command.Id);

            if (product != null)
            {
                product.Name = command.Name ?? product.Name;
                product.Description = command.Description ?? product.Description;
                if (uploadRequest != null)
                {
                    product.ImageDataURL = uploadService.UploadAsync(uploadRequest);
                }


                product.Price = command.Price == 0 ? product.Price : command.Price;
                product.Description = command.Description;
                product.FestivalId = command.FestivalId == 0 ? product.FestivalId : command.FestivalId;
                product.ProductType = (ProductType)command.ProductType;
                product.IsEnable = isAdmin;
                await UpdateFestivalArtCategory(command.ProductImages, command.Id);
                await unitOfWork.Repository<Product>().UpdateAsync(product);
                
                var dbSeoTags = await unitOfWork.Repository<MetaTag>()
                    .Entities.Where(p => p.PageId == product.Id.ToString() && p.Type == PageType.News)
                    .FirstOrDefaultAsync(cancellationToken);
                if (dbSeoTags != null)
                {
                    var mappedUpdateSeoTag = mapper.Map(command.SeoTag, dbSeoTags);
                    await unitOfWork.Repository<MetaTag>().UpdateAsync(mappedUpdateSeoTag);
                }
                else
                {
                    var _mappedSeoTag = mapper.Map<MetaTag>(command.SeoTag);
                    _mappedSeoTag.Type = PageType.Product;
                    _mappedSeoTag.PageId = command.Id.ToString();
                    _mappedSeoTag.PageTitle = command.Name;
                    unitOfWork.Repository<MetaTag>().AddAsync(_mappedSeoTag);
                }

                await mediator.Publish(new AddProductByFestivalEvent()
                {
                    FestivalId = product.FestivalId,
                    ProductId = product.Id,
                    FestivalName = festival.Name
                }, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(product.Id, localize["Product Updated"]);
            }

            return await Result<int>.FailAsync(localize["Product Not Found!"]);
        }
    }

    private async Task UpdateFestivalArtCategory(List<ProductImageDto> clientImages, int productId)
    {
        var dbFestivalArtCategory = await unitOfWork.Repository<ProductImage>().Entities
            .Where(p => p.ProductId == productId)
            .ToListAsync();

        var deletedImages = dbFestivalArtCategory
            .Where(dbImage => clientImages.All(clImage => clImage.Id != dbImage.Id))
            .ToList();

        var updatedImages = dbFestivalArtCategory
            .Where(dbImage => clientImages.Any(clImage => clImage.Id == dbImage.Id))
            .ToList();

        var addedImages = clientImages.Where(clImage => clImage.Id == 0)
            .ToList();

        if (deletedImages != null)
        {
            foreach (var item in deletedImages)
            {
                uploadService.DeleteAsync(new DeleteFileRequest() { RelativeDirectory = item.Url });
                await unitOfWork.Repository<ProductImage>().DeleteAsync(item);
            }
        }

        if (addedImages != null)
        {
            foreach (var item in addedImages)
            {
                var url = uploadService.UploadAsync(item.UploadRequest);
                await unitOfWork.Repository<ProductImage>().AddAsync(new ProductImage()
                {
                    Url = url,
                    ProductId = productId
                });
            }
        }

        if (updatedImages != null)
        {
            foreach (var dbImage in updatedImages)
            {
                var clImage = clientImages.First(p => p.Id == dbImage.Id);
                var updatedUrl = UpdateImage(dbImage.Url, clImage.Url, clImage.UploadRequest);
                dbImage.Url = updatedUrl;
                await unitOfWork.Repository<ProductImage>().UpdateAsync(dbImage);
            }
        }
    }

    private string UpdateImage
        (string dbLogoUrl, string clientLogoUrl, UploadRequest uploadRequest)
    {
        var updatedLogoUrl = dbLogoUrl;
        if (string.IsNullOrWhiteSpace(clientLogoUrl))
        {
            TryDeleteImage(dbLogoUrl);
            updatedLogoUrl = string.Empty;
        }

        if (uploadRequest != null && uploadRequest.Data.Any())
        {
            TryDeleteImage(dbLogoUrl);
            updatedLogoUrl = uploadService.UploadAsync(uploadRequest);
        }

        return updatedLogoUrl;
    }

    private void TryDeleteImage(string dbLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(dbLogoUrl))
        {
            uploadService.DeleteAsync(new DeleteFileRequest { RelativeDirectory = dbLogoUrl });
        }
    }
}