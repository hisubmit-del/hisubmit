using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

namespace HiSubmit.Application.Features.Projects.Queries.GetProjectCreditDetail;

public class GetProjectCreditDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<AddEditProjectCreditItemCommand> ProjectItemPeople { get; set; }

    public int ProjectId { get; set; }
}