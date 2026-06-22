namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterCreateRequest
    {
        public long RequisitionId { get; set; }
        public string FilterName { get; set; } = string.Empty;
        public bool IsAutoShortlistEnabled { get; set; }
        public bool RunOnSubmission { get; set; }
    }
}
