using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Festival.FestivalEditComponent;

public partial class FestivalImages
{
    #region Injection

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback PrevPanel { get; set; }
    [Parameter]public bool IsAdmin { get; set; }

    #endregion

    #region Private Filled

    private List<FestivalImageDto> _images = new();
    private List<FestivalImageDto> _covers = new();
    private bool _processing;
    private bool _loaded;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadFestivalImages();
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadFestivalImages()
    {
        var response = await FestivalManager.GetAllImages(new GetAllFestivalImageQuery()
        {
            FestivalId = FestivalId,
            GetAllData = true
        });
        if (response.Succeeded)
        {
            var imageAndCover = Mapper.Map<List<FestivalImageDto>>(response.Data);
            _images = imageAndCover.Where(p => p.ImageType == ImageType.Images).ToList();
            _covers = imageAndCover.Where(p => p.ImageType == ImageType.Cover).ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _loaded = true;
    }

    public async Task<bool> SaveAsync()
    {
        var result = false;
        _processing = true;
        var festivalImages = _images.Union(_covers);
        var response = await FestivalManager.UploadImages(new AddEditFestivalImageCommand
        {
            FestivalId = FestivalId,
            Images = festivalImages.ToList()
        });
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            StateHasChanged();
            await LoadFestivalImages();
            result = true;
        }
        else
        {
         foreach (var message in response.Messages)
                     _snackBar.Add(message, Severity.Error);   
        }
        

        _processing = false;
        return result;
    }

    private void AddImage()
    {
        _images.Add(new FestivalImageDto { ImageType = ImageType.Images });
    }

    private void AddCover()
    {
        _covers.Add(new FestivalImageDto { ImageType = ImageType.Cover });
    }

    private void DeleteImage(FestivalImageDto image)
    {
        _images.Remove(image);
    }

    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }
    
    private async Task GoPrev()
    {
        await PrevPanel.InvokeAsync();
    }


    public bool ModifiedForm()
    {
        if (_covers.Any(p => p.Id == 0) || _images.Any(p => p.Id == 0))
            return true;
        return false;
    }
}