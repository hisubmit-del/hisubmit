using AutoMapper;
using HiSubmit.Application.Responses.Audit;
using HiSubmit.Infrastructure.Models.Audit;

namespace HiSubmit.Infrastructure.Mappings
{
    public class AuditProfile : Profile
    {
        public AuditProfile()
        {
            CreateMap<AuditResponse, Audit>().ReverseMap();
        }
    }
}