using AutoMapper;
using HiSubmit.Application.Features.MonetaryUnits.Queries;
using HiSubmit.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Mappings
{
    public class MonetaryUnitsProfile:Profile
    {
        public MonetaryUnitsProfile()
        {
            CreateMap<GetAllMonetaryUnitRespnse, MonetaryUnit>().ReverseMap();
        }
    }
}
