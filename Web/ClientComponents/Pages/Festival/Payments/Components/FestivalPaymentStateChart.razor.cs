using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;
using Microsoft.AspNetCore.Components;

namespace ClientComponents.Pages.Festival.Payments.Components;

public partial class FestivalPaymentStateChart
{
    [Parameter]
    public  GetFestivalPaymentStateResponse State { get; set; }
    
    double[] _data = new double[2];
    string[] _labels =new string [2];
    private string _showTitle;
    private string _showTotal;

    protected override Task OnParametersSetAsync()
    {
        _labels[0] = "Paid";
        _labels[1] = "UnPaid";
        _data[0]=(double)State.AdminPayment;
        _data[1] = (double)State.FestivalDebt<0 ? 0:(double)State.FestivalDebt;
        _showTitle = "Paid";
        _showTotal = _data[0] .ToString("C2");
        return base.OnParametersSetAsync();
    }

    private void ChangeTitle(int index)
    {

        _showTitle = _labels[index];
        _showTotal = _data[index].ToString("C2");
    }
    //
    // private void ShowAllTotal()
    //     {
    //         _showTotal = _data.Sum().ToString("C2");
    //         _showTitle = "Total";
    //     }
    
}