using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;

public class AddEditShowHallCommand : IRequest<IResult>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Capacity { get; set; }
    public int AvailableCapacity { get; set; }

    public int VenueId { get; set; }

    public List<ShowTimeDto> ShowTimes { get; set; }

    public AddEditShowHallCommand()
    {
        ShowTimes = new List<ShowTimeDto>();
    }
}

public class AddEditShowHallCommandHandler : IRequestHandler<AddEditShowHallCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddEditShowHallCommandHandler> _localizer;
    private readonly IMapper _mapper;

    public AddEditShowHallCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<AddEditShowHallCommandHandler> localizer, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _mapper = mapper;
    }

    public async Task<IResult> Handle(AddEditShowHallCommand request, CancellationToken cancellationToken)
    {
        request.AvailableCapacity = request.Capacity;
        foreach (var showTime in request.ShowTimes)
        {
            showTime.AvailableCapacity = request.AvailableCapacity;
        }
        if (request.Id == 0)
        {
            var showHall = _mapper.Map<ShowHall>(request);
            showHall.ShowTimes = request.ShowTimes.Select(showTime => _mapper.Map<ShowTime>(showTime)).ToList();
            await _unitOfWork.Repository<ShowHall>().AddAsync(showHall);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(_localizer["show hall added"]);
        }

        //Update show hall
        var dbShowHall =await  _unitOfWork.Repository<ShowHall>().GetByIdAsync(request.Id);
        
        if (dbShowHall == null) return await Result.FailAsync(_localizer["show hall not found"]);
        
        var updatedDbShowHall = _mapper.Map(request, dbShowHall);

        
        await UpdateShowTimes(request.ShowTimes, request.Id,request.Capacity);
        await _unitOfWork.Repository<ShowHall>()
            .UpdateAsync(updatedDbShowHall);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["show hall updated"]);

    }

    private async Task UpdateShowTimes(IReadOnlyCollection<ShowTimeDto> clientShowTime, int showHallId,int showHallCapacity)
    {
        var showTimeIds = clientShowTime.Select(p => p.Id).ToList();

        var showTimesDb = await _unitOfWork.Repository<ShowTime>()
            .Entities.Where(p => p.ShowHallId == showHallId)
            .ToListAsync();

        var deletedShowTimes = showTimesDb
            .Where(showTime => showTimeIds.All(id => id != showTime.Id))
            .ToList();
        var addedShowTimes = clientShowTime
            .Where(clTimeDb =>clTimeDb.Id==0)
            .ToList();
        var updatedShowTimes = clientShowTime
            .Where(showTime => showTimesDb.Any(shDb => shDb.Id == showTime.Id))
            .ToList();

        foreach (var item in deletedShowTimes)
        {
            await _unitOfWork.Repository<ShowTime>().DeleteAsync(item);
        }

        foreach (var addedShowTime in addedShowTimes.Select(item => _mapper.Map<ShowTime>(item)))
        {
            addedShowTime.AvailableCapacity = showHallCapacity;
            await _unitOfWork.Repository<ShowTime>().AddAsync(addedShowTime);
        }

        foreach (var updatedShowTime in updatedShowTimes.Select(showTime => _mapper.Map<ShowTime>(showTime)))
        {
            updatedShowTime.AvailableCapacity = showHallCapacity;
            await _unitOfWork.Repository<ShowTime>().UpdateAsync(updatedShowTime);
        }
    }
    
    
}

public class ShowTimeDto
{
    public int Id { get; set; }
    public  string Name { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }

    public  int AvailableCapacity { get; set; }

    public int ShowHallId { get; set; }


    public override string ToString()
    {
        return $"{Name}:{OpenDate?.Date} {OpenDate?.TimeOfDay}- {OpenDate?.TimeOfDay}";
    }
}
