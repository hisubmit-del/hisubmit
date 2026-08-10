using AutoMapper;
using HiSubmit.Application.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using HiSubmit.Application.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using HiSubmit.Application.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using HiSubmit.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Mappings
{
    public class FestivalFocusProfiile:Profile
    {
        public FestivalFocusProfiile()
        {
            CreateMap<FestivalFocus, AddEditFestivalFocusCommand>().ReverseMap();
            CreateMap<FestivalFocus, GetAllFestivalFocusResponse>().ReverseMap();
            CreateMap<FestivalFocus, GetFestivalFocusDetailResponse>().ReverseMap();
        }
    }
}
