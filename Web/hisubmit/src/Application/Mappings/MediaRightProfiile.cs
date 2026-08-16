using AutoMapper;
using HiSubmit.Application.Features.MediaRights.Queries;
using HiSubmit.Domain.Entities.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Mappings
{
    public class MediaRightProfiile:Profile
    {
        public MediaRightProfiile()
        {
            CreateMap<MediaRight, GetAllMediaRightResponse>().ReverseMap();
        }
    }
}
