using HiSubmit.Client.SharedModels.Constants.Localization;
using HiSubmit.Client.SharedModels.Settings;
using System.Linq;

namespace HiSubmit.Client.Infrastructure.Settings;

public record ClientPreference : IPreference
{
    public bool IsRTL { get; set; }
    public bool IsDarkMode { get; set; }
    public bool IsDrawerOpen { get; set; }
    public string PrimaryColor { get; set; }
    public  int SelectedFestivalId { get; set; }
    public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
}