
using HiSubmit.Client.SharedModels.Settings;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.SharedModels.Managers;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<IResult> ChangeLanguageAsync(string languageCode);
}