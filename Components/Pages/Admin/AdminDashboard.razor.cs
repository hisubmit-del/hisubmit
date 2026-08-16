using HiSubmit.Application.Features.AdminDashboard.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminDashboard;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.Admin;

public partial class AdminDashboard
{
    [Inject]
    private IAdminDashboardManager AdminDashboardManager { get; set; }

    private GetFestivalAndUserStatusCount _statusCount=new();
    private List<GetAllFestivalResponse> _underInvestigationFestivals = new ();

    private GetSitePurchaseResponse _sitePurchase = new ();
    

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadStatusCount();
        await LoadFestivalInvestigation();
        await LoadPurchase();
    }

    private async Task LoadStatusCount()
    {
        var df =await AdminDashboardManager.GetAccountStatusCount();
        if (df.Succeeded)
        {
            _statusCount = df.Data;
        }
    }


    private async Task LoadFestivalInvestigation()
    {
        var df = await AdminDashboardManager.GetAllUnderInvestigationFestivals();
        if (df.Succeeded)
        {
            _underInvestigationFestivals = df.Data;
        }
    }

    private string[] _pieChartLabel1 = ["Submissions", "ServiceFee", "Product", "Ticket" ];

    private double[] _pieChartData1 = new double[5];

    private string[] _pieChartLabel2 = ["ServiceFee", "Website’s product fee", "Website’s Ticket fee", "Gold Account Sales"];

    private double[] _pieChartData2 = new double[5];

    private async Task LoadPurchase()
    {
        var df = await AdminDashboardManager.GetPurchase(new GetSitePurchaseQuery()
        {

        });

        if (df.Succeeded)
        {
            _sitePurchase = df.Data;

           _pieChartData1[0] =(double) _sitePurchase.Submission;
           _pieChartData2[0] =(double) _sitePurchase.ServiceFee;
           
            _pieChartData1[1] =(double)_sitePurchase.ServiceFee;
            _pieChartData2[1] =(double)_sitePurchase.SiteProduct;

            _pieChartData1[2] =(double)_sitePurchase.AllProduct;
            _pieChartData2[2] =(double)_sitePurchase.SiteTicket;

            _pieChartData1[3] = (double)_sitePurchase.AllTicket;
            _pieChartData2[3] = (double)_sitePurchase.GoldAccount;
            
            _loadPurchase = true;
        }
    }

    private bool _loadPurchase;

}