using HiSubmit.Application.Features.DistributionInformations.Commands;
using HiSubmit.Application.Features.Projects.Commands.AddEditAward;
using HiSubmit.Application.Features.Projects.Commands.AddEditProjectCreditCommand;
using HiSubmit.Application.Features.Projects.Commands.AddEditProjectDetail;
using HiSubmit.Application.Features.Projects.Commands.AddEditProjectFileURL;
using HiSubmit.Application.Features.Projects.Commands.DeleteProjectFiles;
using HiSubmit.Application.Features.Projects.Commands.EditProjectSubmitterInformation;
using HiSubmit.Application.Features.Projects.Commands.UpdateScreenWritings;
using HiSubmit.Application.Features.Projects.Commands.UploadProjectFile;
using HiSubmit.Application.Features.Projects.Queries.GetAll;
using HiSubmit.Application.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectCredits;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectFiles;
using HiSubmit.Application.Features.Projects.Queries.GetAwardDetail;
using HiSubmit.Application.Features.Projects.Queries.GetDetail;
using HiSubmit.Application.Features.Projects.Queries.GetProjectFileDetail;
using HiSubmit.Application.Features.Projects.Queries.GetProjectSpecifications;
using HiSubmit.Application.Features.Projects.Queries.GetScreenAward;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Projects.Commands.ProjectImages;
using HiSubmit.Application.Features.Projects.Commands.ReleaseProject;
using HiSubmit.Application.Features.Projects.Commands.UpdateProjectFileOrder;
using HiSubmit.Application.Features.Projects.Queries.GetAllProjectImages;
using HiSubmit.Application.Features.Projects.Queries.GetAllSubProjectType;
using Microsoft.AspNetCore.Authorization;
using HiSubmit.Application.Features.Recommendations.Queries;

namespace Web.Controllers.v1.Project
{
    public class ProjectController : BaseApiController<ProjectController>
    {
        /// <summary>
        /// Get all Projects
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProjectQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GoldFestivalRecommendations")]
        [Authorize]
        public async Task<IActionResult> GoldFestivalRecommendations(
            [FromQuery] GetGoldFestivalRecommendationsQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Release project Confirm(Change project) 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("Release")]
        [Authorize]
        public async Task<IActionResult> Release(ReleaseProjectCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// Get _project detail
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// 
        [HttpGet("Detail")]
        public async Task<IActionResult> GetDetail([FromQuery] GetProjectDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Update _project detail 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateDetail")]
        [Authorize]
        public async Task<IActionResult> UpdateProjectDetail(AddEditProjectDetailCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// update submitter field by default submitter information equal user addedProject iinformation
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateSubmitter")]
        [Authorize]
        public async Task<IActionResult> UpdateSubmitter(EditProjectSubmitterInformationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// add or edit project credit with persons of credit and delete credit and person of credit
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateCredit")]
        [Authorize]
        public async Task<IActionResult> UpdateCredit(UpdateProjectCreditsCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// Get All Credit With Persons or not with parameter withInclude
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("AllCredits")]
        public async Task<IActionResult> GetAllCredit([FromQuery] GetAllProjectCreditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Add Or edit and delete screen award item in the project 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateScreenAwards")]
        [Authorize]
        public async Task<IActionResult> UpdateScreenAward(UpdateScreenWritingCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// Get Screen award item in the project
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("DetailScreenAwards")]
        public async Task<IActionResult> GetScreenAwards([FromQuery] GetScreenAwardQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Add Or edit and delete  award item in the project 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateAwards")]
        [Authorize]
        public async Task<IActionResult> UpdateScreenAward(UpdateAwardCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Get  award item in the project
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("DetailAwards")]
        public async Task<IActionResult> GetAwards([FromQuery] GetAwardDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Add Or edit and delete distribuation information with item in the project 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateDistribuationInformation")]
        [Authorize]
        public async Task<IActionResult> UpdateDistribuationInformation(UpdateDistributionInformationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// Get distribuation information item in the project
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("DetailDistribuationInformation")]
        public async Task<IActionResult> GetdistribuationInformation(
            [FromQuery] GetAllDistribuationInformationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Add Or Edit Project File ProjectURL (local url or not)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("UpdateFileURL")]
        [Authorize]
        public async Task<IActionResult> UpdateProjectFileURl(AddEditProjectFileUrlRequest request)
        {
            return Ok(await Mediator.Send(request));
        }


        /// <summary>
        ///Upload file With chunk 
        /// </summary>
        /// <param name="projectId">id of project files</param>
        /// <param name="fregment">fregment of file</param>
        ///<param name="file">file for updated</param>
        /// <returns></returns>
        [HttpPost("UploadFile")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromQuery] int projectId, [FromQuery] int fregment, IFormFile file)
        {
            return Ok(await Mediator.Send(new UploadProjectFileCommand()
            {
                ProjectId = projectId,
                FormFile = file,
                Fragment = fregment
            }));
        }


        /// <summary>
        /// get all project files with project
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("AllFiles")]
        public async Task<IActionResult> GetAllFiles([FromQuery] GetAllProjectFilesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// get project File Detail (name , description ,password ,... ) for local and unlocal file
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("ProjectFileDetail")]
        public async Task<IActionResult> GetProjectFileDetail([FromQuery] GetProjectFileDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// delete project file local and unlocal if file is local delete file from server 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("DeleteProjectFile")]
        [Authorize]
        public async Task<IActionResult> DeleteProjectFile([FromQuery] DeleteProjectFilesCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetSpecification")]
        public async Task<IActionResult> GetSpecification([FromQuery] GetProjectSpecificationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GetSelectedSpecification")]
        public async Task<IActionResult> GetSelectedSpecification([FromQuery] GetAllSubProjectSelectedTypeQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Add Project image for photography project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("AddProjectImage")]
        [Authorize]
        public async Task<IActionResult> AddProjectImage(AddProjectImageCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Get All Image for project of photography project
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("GetAllProjectImage")]
        public async Task<IActionResult> GetAllImages([FromQuery] GetAllProjectImagesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        
        /// <summary>
        /// Update Project File Orders
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("UpdateProjectFileOrder")]
        [Authorize]
        public async Task<IActionResult> UpdateProjectFileOrder(UpdateProjectFileOrderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
