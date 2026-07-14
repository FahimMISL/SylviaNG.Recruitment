namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterPreviewResponse
    {
        public int TotalApplications { get; set; }
        public int PassingCount { get; set; }
        public List<long> PassingJobApplicationIds { get; set; } = new();
    }
}
