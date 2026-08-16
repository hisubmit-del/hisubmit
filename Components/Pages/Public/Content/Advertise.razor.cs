using MudBlazor;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using HiSubmit.Client.Infrastructure.Managers.Advertises;

namespace Web.Components.Pages.Public.Content;

public partial class Advertise
{
    #region Inject
    [Inject]
    private IAdvertiseManager AdvertiseManager { get; set; }
    #endregion

    #region private Field

    private AddAdvertiseRequest _advertise = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;
    private string _userEmail;
    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await TryGetUserEmail();
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task TryGetUserEmail()
    {
        var currentUser = await AuthenticationManager.CurrentUser();
        var identity = currentUser.Identity;
        if (identity is { IsAuthenticated: true })
        {
            _userEmail = currentUser.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
        }

        _advertise.Email = _userEmail;
    }
    private async Task SaveAsync()
    {
        _processing = true;
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await AdvertiseManager.AddAdvertise(_advertise);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _advertise = new AddAdvertiseRequest();
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }

        _processing = false;
    }

    private void DeleteImage(ImageDto image)
    {
        _advertise.Images.Remove(image);
    }

    private void DeleteFiles(AttachFileDto file)
    {
        _advertise.Files.Remove(file);
    }

    private void AddImage()
    {
        _advertise.Images.Add(new ImageDto());
    }

    private void AddFile()
    {
        _advertise.Files.Add(new AttachFileDto());
    }
}