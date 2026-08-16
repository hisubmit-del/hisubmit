using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Client.Shared.Components
{
    public partial class AddEditCategoryDeadLineFee
    {
        [Parameter]
        public UpdateDeadlineCategoryonFee Fee { get; set; } = new();
    }
}
