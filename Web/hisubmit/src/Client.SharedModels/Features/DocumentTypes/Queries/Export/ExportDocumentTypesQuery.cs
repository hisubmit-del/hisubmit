namespace Hisubmit.Client.SharedModels.Features.DocumentTypes.Queries.Export;

public class ExportDocumentTypesQuery 
{
    public string SearchString { get; set; }

    public ExportDocumentTypesQuery(string searchString = "")
    {
        SearchString = searchString;
    }
}