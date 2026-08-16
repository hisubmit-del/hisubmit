using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Localization;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;

namespace HiSubmit.Application.Features.Wrapper;

public class FeatureBaseService<T> where T : class
{
    protected readonly IMapper _mapper;
    protected readonly IUnitOfWork<int> _unitOfWork;
    protected readonly IStringLocalizer<T> _localize;
    protected readonly ICurrentUserService _currentUserService;
    protected   string CurrentUserId { get;private set; }

    protected FeatureBaseService()
    {
        _mapper = Activator.CreateInstance<IMapper>();
        _unitOfWork = Activator.CreateInstance<IUnitOfWork<int>>();
        _localize = Activator.CreateInstance<IStringLocalizer<T>>();
        _currentUserService = Activator.CreateInstance<ICurrentUserService>();
        LoadUserId();
    }

    protected FeatureBaseService
        (IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<T> localize)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
    }

    private async Task LoadUserId()
    {
        CurrentUserId = _currentUserService.UserId;
    }
}


public enum RequestAccountType : byte
{
    User = 0,
    Festival = 1,
    Admin = 2
}