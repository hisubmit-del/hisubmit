using System.Threading.Tasks;

namespace HiSubmit.Infrastructure.Services;

public interface IViewRenderService
{
    Task<string> RenderViewToStringAsync();
}