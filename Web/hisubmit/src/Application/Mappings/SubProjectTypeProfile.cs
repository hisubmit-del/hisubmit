using AutoMapper;
using HiSubmit.Application.Features.Projects.Queries.GetAllSubProjectType;
using HiSubmit.Application.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Domain.Entities.Projects;


namespace HiSubmit.Application.Mappings
{
    public class SubProjectTypeProfile:Profile
    {
        public SubProjectTypeProfile()
        {
            CreateMap<GetAllSubProjectTypeResponse, SubProjectType>().ReverseMap();



        }
    }
}
