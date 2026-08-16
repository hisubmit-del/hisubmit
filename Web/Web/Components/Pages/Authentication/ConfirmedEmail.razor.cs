using System.Reflection.Metadata;
using Blazored.FluentValidation;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using System.Timers;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Timer = System.Timers.Timer;

namespace Web.Components.Pages.Authentication;

public partial class ConfirmedEmail
{
    [Inject]
    private SelectedAccountService SelectedAccountService { get; set; }
    private FluentValidationValidator _fluentValidationValidator;
    private bool _validated = true;
    private VerificationCodeRequest _model = new();
    private bool _processing;
    private Timer timer;
    private bool _enableButton;


    protected override async Task OnInitializedAsync()
    {
        StartTimer();

        var state = await _stateProvider.GetAuthenticationStateAsync();

        var isAuthenticated = state.User.Identity!.IsAuthenticated;

        if (isAuthenticated || !await _localStorage.ContainKeyAsync(StorageConstants.Local.EmailRegistered))
        {
            _navigationManager.NavigateTo("/", false);
        }

        _model.Email = await _localStorage.GetItemAsync<string>(StorageConstants.Local.EmailRegistered);

        await base.OnInitializedAsync();
    }


    private void EnableButton(object? sender, ElapsedEventArgs e)
    {
        _enableButton = true;
        InvokeAsync(StateHasChanged); // به‌روزرسانی رابط کاربری
        timer.Stop();
    }

    private async Task SubmitAsync()
    {
        _processing = true;

        _validated= _fluentValidationValidator
            .Validate(options => { options.IncludeAllRuleSets(); });


        var result = await AuthenticationManager.VerifyEmail(_model);
        if (result.Succeeded)
        {
            _snackBar.Add(string.Format(Localize["Your Email Confirmed"]),
                Severity.Success);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.EmailRegistered);
            _navigationManager.NavigateTo("/", false);
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
        _processing = false;
    }

    public async Task ResendEmail()
    {
        var response = await AuthenticationManager.ResendVerifyEmail(new
            ResendVerificationCodeRequest()
        {
            Email = _model.Email
        });

        if (response.Succeeded)
            _snackBar.Add("The activation email has been sent to you.", Severity.Success);


        ResetTimer();
    }

    private void StartTimer()
    {
        CountdownMessage = $"Button will be enabled in {Countdown} seconds.";
        timer = new Timer(1000); // اجرای هر ثانیه
        timer.Elapsed += UpdateCountdown;
        timer.Start();
    }

    public string CountdownMessage { get; set; }

    private void UpdateCountdown(object? sender, ElapsedEventArgs e)
    {
        if (Countdown > 0)
        {
            Countdown--;
            CountdownMessage = $"Resend Button will be enabled in {Countdown} seconds.";
            InvokeAsync(StateHasChanged); // به‌روزرسانی رابط کاربری
        }
        else
        {
            EnableButton();
        }
    }

    public int Countdown { get; set; } = 120;

    private void EnableButton()
    {
        _enableButton = true;
        CountdownMessage = "";
        InvokeAsync(StateHasChanged);
        timer.Stop();
    }

    private void ResetTimer()
    {
        timer.Stop();
        Countdown = 120; // بازنشانی شمارش معکوس
        _enableButton = false;// غیرفعال کردن دکمه
        StartTimer(); // شروع مجدد تایمر
        InvokeAsync(StateHasChanged);
    }

}
