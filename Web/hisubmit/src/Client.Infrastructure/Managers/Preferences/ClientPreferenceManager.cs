//using Blazored.LocalStorage;
using HiSubmit.Client.Infrastructure.Settings;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Settings;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Preferences;

public class ClientPreferenceManager : IClientPreferenceManager
{
    //private readonly ILocalStorageService _localStorageService;
    private readonly IStringLocalizer<ClientPreferenceManager> _localize;

    public ClientPreferenceManager(
        //ILocalStorageService localStorageService,
        IStringLocalizer<ClientPreferenceManager> localize)
    {
        //_localStorageService = localStorageService;
        _localize = localize;
    }

    public async Task<bool> ToggleDarkModeAsync()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsDarkMode = !preference.IsDarkMode;
            await SetPreference(preference);
            return !preference.IsDarkMode;
        }

        return false;
    }
    public async Task<bool> ToggleLayoutDirection()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsRTL = !preference.IsRTL;
            await SetPreference(preference);

            return preference.IsRTL;
        }
        return false;
    }

    public async Task<IResult> ChangeLanguageAsync(string languageCode)
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.LanguageCode = languageCode;
            await SetPreference(preference);
            return new Result
            {
                Succeeded = true,
                Messages = new List<string> { _localize["Client Language has been changed"] }
            };
        }

        return new Result
        {
            Succeeded = false,
            Messages = new List<string> { _localize["Failed to get client preferences"] }
        };
    }

    public async Task<MudTheme> GetCurrentThemeAsync()
    {
        return await GetPreference() is ClientPreference 
            { IsDarkMode: true } ? BlazorHeroTheme.DefaultTheme : BlazorHeroTheme.DefaultTheme;
    }
    public async Task<bool> IsRtl()
    {
        if (await GetPreference() is not ClientPreference preference) return false;
        return preference.IsDarkMode != true && preference.IsRTL;
    }

    public async Task<IPreference> GetPreference()
    {
        
        return
            //await _localStorageService.GetItemAsync<ClientPreference>(StorageConstants.Local.Preference)
               //??
            new ClientPreference();
    }

    public async Task SetPreference(IPreference preference)
    {
        
        //await _localStorageService.SetItemAsync
        //    (StorageConstants.Local.Preference, preference as ClientPreference);
    }

    public async Task SetRtl()
    {
        if(await GetPreference() is ClientPreference preference)
        {
            preference.IsRTL = true;
            await SetPreference(preference);
        }
    }
    public async Task SetLtr()
    {
        if (await GetPreference() is ClientPreference preference)
        {
            preference.IsRTL = false;
            await SetPreference(preference);
        }
    }
}
