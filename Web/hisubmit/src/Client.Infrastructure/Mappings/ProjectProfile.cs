using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditMusicSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditPhotographySpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditScriptSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditVrXrSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.EditProjectSubmitterInformation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;

namespace HiSubmit.Client.Infrastructure.Mappings
{
    public class ProjectProfile:Profile
    {
        public ProjectProfile()
        {
            CreateMap<AddEditProjectDetailCommand, GetProjectDetailResponse>().ReverseMap();
            CreateMap<EditProjectSubmitterInformationCommand,GetProjectDetailResponse>().ReverseMap();

            CreateMap<AddEditProjectCreditCommand, GetAllProjectCreditResponse>().ReverseMap();

            //Specification
            CreateMap<AddEditFilmSpecificationCommand, GetFilmSpecificationDetailResponse>()
                .ForMember(des=>des.SubProjectTypeIds,map=>map.MapFrom(src=>src.SubProjectTypeIds)).ReverseMap();
            CreateMap<AddEditScriptSpecificationCommand, GetScriptSpecificationDetailResponse>().ReverseMap();
            CreateMap<AddEditMusicSpecificationCommand, GetMusicSpecificationDetailResponse>().ReverseMap();
            CreateMap<AddEditPhotographySpecificationCommand, GetPhotographySpecificationDetailResponse>().ReverseMap();
            CreateMap<AddEditVrXrSpecificationCommand, GetVrXrSpecificationDetailResponse>().ReverseMap();
            CreateMap<AddEditAwardRequest, GetAwardDetailResponse>().ReverseMap();
            CreateMap<AddEditScreenWritingRequest, GetScreenAwardResponse>().ReverseMap();

            //File
            CreateMap<AddEditProjectFileURLRequest, GetProjectDetailResponse>().ReverseMap();
        }
    }
}