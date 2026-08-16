using AutoMapper;
using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgiingButton;
using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgingButton;
using HiSubmit.Application.Features.Judgings.Queries.Detail;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Application.Mappings
{
    public class JudgingProfile:Profile
    {
        public JudgingProfile()
        {
            CreateMap<AddEditJudgingButtonCommand, JudgingButton>().ReverseMap();
            CreateMap<AddEditJudgingFiledCommand, JudgingFiled>().ReverseMap();
            CreateMap<Judging, GetJudgingDetailResponse>()
                .ForMember(p => p.Questions, map => map.MapFrom(p => p.SubmissionQuestions));
        }
    }
}
