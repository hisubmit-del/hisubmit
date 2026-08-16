namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail
{
    public class GetMusicSpecificationDetailQuery
    {
        public int ProjectId { get; set; }
    }

    public class GetMusicSpecificationDetailResponse
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }
        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime CompletionDate { get; set; }
        public int OriginCountryId { get; set; }

        public string Language { get; set; }

        public bool StudentProject { get; set; }

        //navigation Property
        public int ProjectId { get; set; }
        public string OriginCountryName { get; set; }

        public GetMusicSpecificationDetailResponse()
        {
            CompletionDate = DateTime.Today;
        }
    }
}
