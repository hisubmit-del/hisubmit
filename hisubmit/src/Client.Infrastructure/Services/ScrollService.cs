using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace HiSubmit.Client.Infrastructure.Services;

public class ScrollService(IJSRuntime jsRuntime)
{
    public async Task ScrollToId(string id)
    {
        await jsRuntime.InvokeVoidAsync("ScrollToElement",$"#{id}");
    }
}