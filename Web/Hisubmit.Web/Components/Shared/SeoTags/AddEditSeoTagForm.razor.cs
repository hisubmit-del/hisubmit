using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Seo;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Web.Components.Shared.SeoTags;

public partial class AddEditSeoTagForm
{
    [Parameter] public AddEditSeoTagRequest Model { get; set; } = new();
}