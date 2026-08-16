using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.RenderView;

public interface IRenderViewService
{
    Task<string> RenderViewToStringAsync(string viewName,string folderName=null);
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model,string folderName=null);
}
