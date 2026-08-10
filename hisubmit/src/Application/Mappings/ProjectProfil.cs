using AutoMapper;
using System.Linq;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Application.Features.DistributionInformations.Commands;
using HiSubmit.Application.Features.Projects.Commands.AddEditFilmSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditMusicSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditPhotographySpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditProjectDetail;
using HiSubmit.Application.Features.Projects.Commands.AddEditProjectFileURL;
using HiSubmit.Application.Features.Projects.Commands.AddEditScriptSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditVrXrSpecification;
using HiSubmit.Application.Features.Projects.Commands.EditProjectSubmitterInformation;
using HiSubmit.Application.Features.Projects.Queries.GetAll;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectCredits;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectFiles;
using HiSubmit.Application.Features.Projects.Queries.GetDetail;
using HiSubmit.Application.Features.Projects.Queries.GetMusicSpecificationDetail;
using HiSubmit.Application.Features.Projects.Queries.GetProjectCreditDetail;
using HiSubmit.Application.Features.Projects.Queries.GetProjectSpecifications;
using HiSubmit.Application.Features.Projects.Queries.GetVrXrSpecificationDetail;
using HiSubmit.Application.Features.Projects.Commands.ProjectImages;
using HiSubmit.Application.Features.Projects.Queries.GetAllSubProjectType;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectImages;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using GetScriptSpecificationDetailResponse = HiSubmit.Application.Features.Projects.Queries.GetFilmSpecificationDetail.GetScriptSpecificationDetailResponse;

namespace HiSubmit.Application.Mappings
{
    public class ProjectProfile:Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, GetAllProjectResponse>().ReverseMap();
            CreateMap<Project, GetProjectDetailResponse>().ReverseMap();
            CreateMap<Project, AddEditProjectDetailCommand>().ReverseMap();
            CreateMap<Project, EditProjectSubmitterInformationCommand>().ReverseMap();

            //project credit
            CreateMap<ProjectCredit, GetAllProjectCreditResponse>().ReverseMap();
            CreateMap<ProjectCredit, AddEditProjectCreditCommand>().ReverseMap();
            CreateMap<ProjectItemPerson, AddEditProjectCreditItemCommand>().ReverseMap();
            CreateMap<ProjectCredit, GetProjectCreditDetailResponse>().ReverseMap();

            //Specification
            CreateMap<FilmSpecification, AddEditFilmSpecificationCommand>().ReverseMap();
            CreateMap<FilmSpecification, GetFilmSpecificationDetailResponse>()
                .ForMember(des => des.SubProjectTypeIds,
                    map => map.MapFrom(src => src.ProjectTypes.Select(p => p.SubProjectTypeId)))
                .ForMember(des => des.FilmingCountryIds,
                    map => map.MapFrom(src => src.FilmingCountries.Select(p => p.Id)))
                .ForMember(des => des.FilmingCountriesName,
                    map => map.MapFrom(src => src.FilmingCountries.Select(p => p.Name)));

            CreateMap<MusicSpecification, AddEditMusicSpecificationCommand>().ReverseMap();
            CreateMap<MusicSpecification, GetMusicSpecificationDetailResponse>()
                .ForMember(des=>des.SubProjectTypeIds,map=>map.MapFrom(src=>src.ProjectType.Select(p=>p.SubProjectTypeId)));

            CreateMap<ScriptSpecification, AddEditScriptSpecificationCommand>().ReverseMap();
            CreateMap<ScriptSpecification, GetScriptSpecificationDetailResponse>()
                .ForMember(des => des.SubProjectTypeIds, map => map.MapFrom(src => src.ProjectTypes.Select(p => p.SubProjectTypeId)));


            CreateMap<XrVrSpecification, AddEditVrXrSpecificationCommand>().ReverseMap();
            CreateMap<XrVrSpecification, GetVrXrSpecificationDetailResponse>()
                .ForMember(des => des.SubProjectTypeIds, map => map.MapFrom(src => src.ProjectType.Select(p => p.SubProjectTypeId)));


            CreateMap<PhotographySpecification, AddEditPhotographySpecificationCommand>().ReverseMap();
            CreateMap<PhotographySpecification, GetPhotographySpecificationDetailResponse>()
                .ForMember(des => des.SubProjectTypeIds, map => map.MapFrom(src => src.PhotographySpecificationSubProjectTypes.Select(p => p.SubProjectTypeId)));


            CreateMap<AddEditDistributionInformationItemRequest, DistributionInformationItem>().ReverseMap()
                .ForMember(p=>p.MediaRightIds,map=>map.MapFrom(src=>src.MediaRightDistributionInformation.Select(p=>p.MediaRightId)));

            CreateMap<AddEditDistributionInformationRequest, DistributionInformation>()
                .ForMember(p => p.Items, map => map.Ignore());

            CreateMap<DistributionInformation, AddEditDistributionInformationRequest>();

            CreateMap<AddEditScreenWritingRequest, ScreeningAward>().ReverseMap();
            CreateMap<AddEditAwardRequest, Award>().ReverseMap();

            CreateMap<ScreeningAward, GetScreenAwardResponse>().ReverseMap();
            CreateMap<Award, GetAwardDetailResponse>().ReverseMap();



            CreateMap<MonetaryUnitDto, MonetaryUnit>().ReverseMap();
            //Project file
            CreateMap<AddEditProjectFileUrlRequest, ProjectFile>().ReverseMap();
            CreateMap<GetAllProjectFileResponse, ProjectFile>().ReverseMap();



            //Specification
            CreateMap<Project, GetProjectSpecificationResponse>().ReverseMap();
            
            CreateMap<SubProjectTypeFilmSpecification, SubProjectSpecficationDto>().ReverseMap();
            CreateMap<PhotographySpecificationSubProjectType, SubProjectSpecficationDto>().ReverseMap();
            CreateMap<SubProjectTypeMusicSpecification, SubProjectSpecficationDto>().ReverseMap();
            CreateMap<SubProjectTypeScriptSpecificaion, SubProjectSpecficationDto>().ReverseMap();
            CreateMap<SubProjectTypeVRXrSpecification, SubProjectSpecficationDto>().ReverseMap();
            
            
            //project photo
            CreateMap<AddProjectImageCommand, ProjectImage>().ReverseMap();
            CreateMap<GetAllProjectImageResponse,ProjectImage>().ReverseMap();
        }
    }
}
