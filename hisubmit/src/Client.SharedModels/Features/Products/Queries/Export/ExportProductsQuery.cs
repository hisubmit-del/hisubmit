namespace Hisubmit.Client.SharedModels.Features.Products.Queries.Export;

public class ExportProductsQuery 
{
    public string SearchString { get; set; }
    public int FestivalId { get; set; }
    public bool? IsEnable { get; set; }

    public ExportProductsQuery(string searchString = "")
    {
        SearchString = searchString;
    }
}