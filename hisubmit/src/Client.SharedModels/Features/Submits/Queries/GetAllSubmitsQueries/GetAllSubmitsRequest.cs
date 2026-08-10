using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries
{
    public class GetAllSubmitsRequest : PagedRequest
    {
        public string UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? FestivalId { get; set; }
        public string SearchString { get; set; }
        public string TrackingCode { get; set; }
        public string FestivalName { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime? SubmitDateFrom { get; set; }
        public DateTime? SubmitDateTo { get; set; }
        public bool GetCurrentUserSubmits { get; set; }
        
        public SubmitStatus? SubmitStatus { get; set; }
        public JudgingStatus? JudgingStatus { get; set; }
    }
}