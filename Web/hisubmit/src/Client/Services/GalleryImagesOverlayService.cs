using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;

namespace HiSubmit.Client.Services;

public class GalleryImagesOverlayService
{
    public ImageDto[] Images { get; set; } = [];

    public ImageDto SelectedImage { get; set; }

    public event EventHandler OnOverlayImageChanged;

    public bool Visible { get; set; }

    public Task ShowImageOverlay(ImageDto[] images, ImageDto selectedImage)
    {
        Images = images;
        SelectedImage = selectedImage;
        Visible= true;
        OnOverlayImageChanged?.Invoke(this, new EventArgs());
        return
            Task.CompletedTask;
    }

    public void NextImages()
    {
        var currentIndex = Array.IndexOf(Images, SelectedImage);
        var nextIndex = 0;
        if (currentIndex == Images.Length-1)
        {
            nextIndex = 0;
        }
        else
        {
            nextIndex = currentIndex+1;
        }
        SelectedImage=Images[nextIndex];
    }

    public void PrevImages()
    {
        var currentIndex = Array.IndexOf(Images, SelectedImage);
        var prevIndex = 0;
        if (currentIndex == 0)
        {
            prevIndex = Images.Length-1;
        }
        else
        {
            prevIndex = currentIndex-1;
        }
        SelectedImage=Images[prevIndex];
    }
}