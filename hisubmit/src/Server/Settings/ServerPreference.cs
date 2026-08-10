using HiSubmit.Client.SharedModels.Constants.Localization;
using HiSubmit.Client.SharedModels.Settings;
using System.Linq;

namespace HiSubmit.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";

        //TODO - add server preferences
    }
}