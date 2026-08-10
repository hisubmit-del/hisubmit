using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail
{
    public class GetFestivalFileDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileURL { get; set; }
        public FileFormat FileFormat { get; set; }
        public int FestivalId { get; set; }
    }
}
