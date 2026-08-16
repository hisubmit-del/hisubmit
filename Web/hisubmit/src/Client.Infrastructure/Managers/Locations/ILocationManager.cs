using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Locations
{
    public interface ILocationManager:ITransientManager
    {
        Task<Result<List<GetAllCountryResponse>>> GetAllCountryAsync(GetAllCountryQuery query);
    }
}
