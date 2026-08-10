namespace Hisubmit.Client.SharedModels.Features.Documents.Queries.GetAll;

public class GetAllDocumentsQuery 
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string SearchString { get; set; }

    public GetAllDocumentsQuery(int pageNumber, int pageSize, string searchString)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchString = searchString;
    }
}