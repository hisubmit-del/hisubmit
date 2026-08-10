//using Microsoft.AspNetCore.Components;
//using MudBlazor;
//using System.Collections.Generic;
//using System.Reflection.Metadata;
//using System.Threading.Tasks;

//namespace HiSubmit.Hisubmit.Client.SharedModels.Components
//{
//    public partial class CheckBox
//    {
//        [Parameter]
//        public bool IsSelected { get; set; }
//        [Parameter]
//        public int Value{ get; set; }

//        [Parameter]
//        public List<int> ListItems { get; set; }

//        MudCheckBox<bool> checkBox;
//        public EventCallback<int> CheckedChangeItem { get; set; }

//        protected override Task OnInitializedAsync()
//        {
//            checkBox = new MudCheckBox<bool>();
//            //checkBox.Checked = IsSelected;
//            return base.OnInitializedAsync();   
//        }


//        public async Task OnCheckedChange()
//        {
//            //if (checkBox.Checked)
//            //{
//            //    checkBox.Checked = false;
//            //    ListItems.Remove(Value);
//            //}
//            //else
//            //{
//            //    checkBox.Checked = true;
//            //    ListItems.Add(Value);
//            //}
//            //await CheckedChangeItem.InvokeAsync(Value);
//        }
//    }
//}
