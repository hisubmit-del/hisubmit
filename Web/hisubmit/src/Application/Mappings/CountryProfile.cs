using AutoMapper;
using HiSubmit.Application.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Domain.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Mappings
{
    public class CountryProfile:Profile
    {
        public CountryProfile()
        {
            CreateMap<Country, GetAllCountryResponse>();
        }
    }
}
