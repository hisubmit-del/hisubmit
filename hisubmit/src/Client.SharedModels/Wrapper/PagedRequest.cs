namespace Hisubmit.Client.SharedModels.Wrapper;

public abstract class PagedRequest
{
    public string SearchString { get; set; }
    
    public bool GetAllData { get; set; }

    public string[]? Orderby { get; set; }
    
    public bool OrderByAscending { get; set; }
    

    
    private int _pageSize;

    public int PageSize
    {
        get
        {
            if (GetAllData)
            {
                return 10000;
            }

            return _pageSize;
        }
        set
        {
            if (value <= 0)
            {
                _pageSize = 10;
            }
            else
            {
                _pageSize = value;
            }
        }
    }

    private int _pageNumber;

    public int PageNumber
    {
        get
        {
            if (GetAllData)
            {
                return 1;
            }

            return _pageNumber;
        }
        set
        {
            if (value <= 0)
            {
                _pageNumber = 1;
            }
            else
            {
                _pageNumber = value;
            }
        }
    }


    public int GetSkipCount()
    {
        return (PageNumber - 1) * PageSize;
    }
}