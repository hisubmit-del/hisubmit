namespace HiSubmit.Application.Features.Festivals.Commands.AddEditAdditinalSettings
{
    public class UpdateFestivalArtCategory
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
        public int ArtCategoryId { get; set; }
        public string ArtCategoryName { get; set; }
    }
}
