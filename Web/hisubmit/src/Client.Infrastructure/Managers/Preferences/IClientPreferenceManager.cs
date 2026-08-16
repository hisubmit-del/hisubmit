using HiSubmit.Client.SharedModels.Managers;
using MudBlazor;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}