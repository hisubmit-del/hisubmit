using AdminDashboard.Wasm.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HiSubmit.Web.Components.Shared.Components
{
    public partial class GroupCheckBox
    {
        [Parameter]
        public List<CheckBoxItem<int>> Items { get; set; } = new();


        public List<int> SelectedItems => Items.Where(p => p.IsSelected).Select(p => p.Value).ToList();

    }
}
