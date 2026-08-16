using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminDashboard.Wasm.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Shared.Components.UserControllers;

public partial class CheckBoxList<T,TItem> where T : struct
{
    [Parameter] public List<T> SelectedItems { get; set; } = new();

    [Parameter] public EventCallback<List<T>> SelectedItemsChanged { get; set; }
    
    [Parameter]public string SelectedItemsString { get; set; }
    [Parameter]public EventCallback<string> SelectedItemsStringChanged { get; set; }

    [Parameter] public List<TItem> Items { get; set; } = new();

    private List<CheckBoxItem<T>> _checkBoxItems = new();
    protected override Task OnInitializedAsync()
    {
        _checkBoxItems = CheckBoxItem<T>.CovertToCheckboxItems(Items).ToList();
        
        return base.OnInitializedAsync();
    }

    private Task SetToSelectedItems(bool b, CheckBoxItem<T> item)
    {
        if (b)
        {
            if (!SelectedItems.Any(p => Equals(p, item.Value)))
                SelectedItems.Add(item.Value);
        }
        else
        {
            if (SelectedItems.Any(p => Equals(p, item.Value)))
                SelectedItems.Remove(item.Value);
        }

        item.IsSelected = b;
        SelectedItemsString = string.Join(',',SelectedItems);
        
        SelectedItemsChanged.InvokeAsync(SelectedItems);
        SelectedItemsStringChanged.InvokeAsync(SelectedItemsString);
        return Task.CompletedTask;
    }
    
}