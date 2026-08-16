using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Locatuions.Countries.Queries.GetAll
{
    public class GetAllCountryQuery:IRequest<Result<List<GetAllCountryResponse>>>
    {

    }
    public class GetAllCountryQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        : IRequestHandler<GetAllCountryQuery, Result<List<GetAllCountryResponse>>>
    {
        //private readonly IAppCache _appCache;
        //,IAppCache appCache
        //_appCache = appCache;

        public async Task<Result<List<GetAllCountryResponse>>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            var res=await unitOfWork.Repository<Country>().GetAllAsync();

            //var countryList = await _appCache.GetOrAddAsync(ApplicationConstants.Cache.GetAllCountryCachKey, getAllCountries);
            var mappedCountry = mapper.Map<List<GetAllCountryResponse>>(res);
            return await Result<List<GetAllCountryResponse>>.SuccessAsync(mappedCountry);
        }
    }
}
