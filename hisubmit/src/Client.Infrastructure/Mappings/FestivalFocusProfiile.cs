using AutoMapper;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Mappings
{
    public class FestivalFocusProfile:Profile
    {
        public FestivalFocusProfile()
        {
            CreateMap<AddEditFestivalFocusCommand, GetFestivalFocusDetailResponse>().ReverseMap();
        }
    }
}
