namespace Hisubmit.Client.SharedModels.Features.Brands.Queries.Export;

public class ExportBrandsRequest 
{
    public string SearchString { get; set; }

    public ExportBrandsRequest(string searchString = "")
    {
        SearchString = searchString;
    }
}