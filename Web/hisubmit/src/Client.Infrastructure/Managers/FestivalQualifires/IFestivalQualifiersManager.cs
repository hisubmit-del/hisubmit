using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalQualifires
{
    public  interface IFestivalQualifiersManager:ITransientManager
    {
        Task<IResult<List<GetAllFestivalQualifiersResponse>>> GetAllAsync(GetAllFestivalQualifiersQuery query);
    }
}

