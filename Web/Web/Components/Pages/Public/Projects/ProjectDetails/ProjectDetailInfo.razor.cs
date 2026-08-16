using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class ProjectDetailInfo
{
    [Parameter] 
    public GetProjectDetailResponse Project { get; set; }
    [Parameter]
    public  List<GetAllProjectCreditResponse> Credits { get; set; }
[Parameter]public bool DetailLoaded { get; set; }
    private bool _originalLanguage;
}