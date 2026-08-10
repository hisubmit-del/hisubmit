using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributionInformation = HiSubmit.Domain.Entities.Projects.DistributionInformation;

namespace HiSubmit.Application.Features.DistributionInformations.Commands;

public class UpdateDistributionInformationCommand : IRequest<Result<int>>
{
    public int ProjectId { get; set; }
    public List<AddEditDistributionInformationRequest> Information { get; set; }
    
    public UpdateDistributionInformationCommand()
    {
        Information = new List<AddEditDistributionInformationRequest>();
    }
}

internal class UpdateDistributionInformationCommandHandler :
        IRequestHandler<UpdateDistributionInformationCommand,
        Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<UpdateDistributionInformationCommandHandler> _localizer;


    public UpdateDistributionInformationCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,
        IStringLocalizer<UpdateDistributionInformationCommandHandler> localizer)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(UpdateDistributionInformationCommand request,
        CancellationToken cancellationToken)
    {
        if (!request.Information.Any())
        {
            await DeleteAllProjectDistribution(request.ProjectId,cancellationToken);
            return await Result<int>.SuccessAsync(_localizer["Updated Distribution Successfully"]);
        }
        var clientIds = request.Information.Select(p => p.Id).ToList();

        var deletedInformation = _unitOfWork.Repository<DistributionInformation>()
            .Entities.Where(p => clientIds.All(id => id != p.Id) && p.ProjectId == request.ProjectId);

        foreach (var info in request.Information)
        {
            foreach (var item in info.Items)
            {
                item.DistributionInformationId = info.Id;
            }
        }
        if (deletedInformation.Any())
        {
            foreach (var credit in deletedInformation)
            {
                await _unitOfWork.Repository<DistributionInformation>().DeleteAsync(credit);
            }
        }

        foreach (var informationRequest in request.Information)
        {
            if (informationRequest.Id == 0)
            {
                var info = _mapper.Map<DistributionInformation>(informationRequest);
                foreach (var item in informationRequest.Items)
                {
                    var informationItem = _mapper.Map<DistributionInformationItem>(item);
                    foreach (var rightId in item.MediaRightIds)
                    {
                        informationItem.MediaRightDistributionInformation.Add(
                            new MediaRightDistributionInformation()
                            {
                                MediaRightId = rightId
                            });
                    }

                    info.Items.Add(informationItem);
                }

                await _unitOfWork.Repository<DistributionInformation>().AddAsync(info);
            }
            else
            {
                var infoIds = informationRequest.Items.Select(p => p.Id);

                var deletedItems = await _unitOfWork.Repository<DistributionInformationItem>()
                    .Entities
                    .Where(p => infoIds.All(id => id != p.Id) &&
                                p.DistributionInformationId == informationRequest.Id)
                    .ToListAsync(cancellationToken);


                foreach (var item in deletedItems)
                {
                    await _unitOfWork.Repository<DistributionInformationItem>().DeleteAsync(item);
                }

                var dbInformation = await
                    _unitOfWork.Repository<DistributionInformation>()
                        .GetByIdAsync(informationRequest.Id);

                if (dbInformation == null)
                {
                    return await Result<int>.FailAsync(_localizer["Error  in update information"]);
                }

                var updatedInformation = _mapper.Map(informationRequest, dbInformation);

                foreach (var info in informationRequest.Items)
                {
                    var informationItem = _mapper.Map<DistributionInformationItem>(info);
                    informationItem.DistributionInformationId = informationRequest.Id;
                    if (info.Id == 0)
                    {
                        foreach (var rightId in info.MediaRightIds)
                        {
                            informationItem.MediaRightDistributionInformation.Add(
                                new MediaRightDistributionInformation()
                                {
                                    MediaRightId = rightId
                                });
                        }

                        updatedInformation.Items.Add(informationItem);
                    }
                    else
                    {
                        await UpdateMediaRight(info.MediaRightIds, info.Id);
                        await _unitOfWork.Repository<DistributionInformationItem>().UpdateAsync(informationItem);
                    }
                }

                await _unitOfWork.Repository<DistributionInformation>().UpdateAsync(updatedInformation);
            }
        }

        await _unitOfWork.CommitAndRemoveCache(cancellationToken);
        return await Result<int>.SuccessAsync(0, _localizer["Information updated"]);
    }

    private async Task DeleteAllProjectDistribution(int requestProjectId,CancellationToken cancellationToken)
    {
        var distribution = await _unitOfWork.Repository<DistributionInformation>()
            .Entities
            .Where(p => p.ProjectId == requestProjectId)
            .Include(p=>p.Items)
            .ToListAsync(cancellationToken);
        var distributionItemsId = distribution.Select(p => p.Id).ToList();
        var mediaRightDistribution = await _unitOfWork.Repository<MediaRightDistributionInformation>()
            .Entities
            .Where(p => distributionItemsId.Any(k => k == p.DistributionInformationItemId))
            .ToListAsync(cancellationToken);
        foreach (var right in mediaRightDistribution)
        {
            await _unitOfWork.Repository<MediaRightDistributionInformation>().DeleteAsync(right);
        }

        await  _unitOfWork.SaveChangesAsync(cancellationToken);
        foreach (var dis in distribution)
        {
            foreach (var item in dis.Items)
            {
                await _unitOfWork.Repository<DistributionInformationItem>().DeleteAsync(item);
            }
            await  _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.Repository<DistributionInformation>().DeleteAsync(dis);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateMediaRight(List<int> rightIds, int informationItemId)
    {
        
        var dbMediaRightDisInformation = _unitOfWork.Repository<MediaRightDistributionInformation>().Entities
            .Where(p => p.DistributionInformationItemId == informationItemId);

        var deletedMediaRightDestribuation = dbMediaRightDisInformation
            .Where(deadlneCat => rightIds.All(id => id != deadlneCat.Id))
            .ToList();

        var addedMediaRightDestribuation = rightIds
            .Where(id => !dbMediaRightDisInformation.Any(focus => focus.Id == id))
            .ToList();

        foreach (var item in deletedMediaRightDestribuation)
        {
            await _unitOfWork.Repository<MediaRightDistributionInformation>().DeleteAsync(item);
        }


        foreach (var item in addedMediaRightDestribuation)
        {
            await _unitOfWork.Repository<MediaRightDistributionInformation>().AddAsync(
                new MediaRightDistributionInformation()
                {
                    DistributionInformationItemId = informationItemId,
                    MediaRightId = item
                });
        }
    }
}