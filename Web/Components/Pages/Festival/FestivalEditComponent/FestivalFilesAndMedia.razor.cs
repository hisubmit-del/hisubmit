using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Pages.Festival.FestivalEditComponent;

public partial class FestivalFilesAndMedia
{
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public bool IsAdmin { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback PrevPanel { get; set; }

    private FestivalFile _files;
    private FestivalImages _media;
    private bool _saving;

    public async Task<bool> SaveAsync()
    {
        if (IsAdmin || _media is null || !_media.ModifiedForm())
            return true;

        _saving = true;
        try
        {
            return await _media.SaveAsync();
        }
        finally
        {
            _saving = false;
        }
    }

    public bool ModifiedForm() => _media?.ModifiedForm() == true;

    public bool ValidateRequiredMedia()
    {
        if (IsAdmin || _media?.HasCover() == true)
            return true;

        _snackBar.Add("A festival banner/cover image is required.", MudBlazor.Severity.Error);
        return false;
    }

    private Task GoNext() => NextPanel.InvokeAsync();
    private Task GoPrev() => PrevPanel.InvokeAsync();
}
