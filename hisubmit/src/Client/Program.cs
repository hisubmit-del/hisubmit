//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;
//using HiSubmit.Client.Extensions;
//using HiSubmit.Client.Infrastructure.Settings;
//using Microsoft.Extensions.DependencyInjection;
//using HiSubmit.Client.Infrastructure.Managers.Preferences;
//using HiSubmit.Client.SharedModels.Constants.Localization;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//namespace HiSubmit.Client;

//public static class Program
//{
//    public static async Task Main(string[] args)
//    {
//        var builder = WebAssemblyHostBuilder
//            .CreateDefault(args)
//            .AddRootComponents()
//            .AddClientServices()
//            .SetLogger();
//        // builder.RootComponents.Add<HeadOutlet>("head::after");
//        var host = builder.Build();
//        var storageService = host.Services.GetRequiredService<ClientPreferenceManager>();
           
//        if (storageService != null)
//        {
//            CultureInfo culture;
//            if (await storageService.GetPreference() is ClientPreference preference)
//                culture = new CultureInfo(preference.LanguageCode);
//            else
//                culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code 
//                                          ?? "en-US");
//            CultureInfo.DefaultThreadCurrentCulture = culture;
//            CultureInfo.DefaultThreadCurrentUICulture = culture;
//        }
//        await builder.Build().RunAsync();
//    }
//}
