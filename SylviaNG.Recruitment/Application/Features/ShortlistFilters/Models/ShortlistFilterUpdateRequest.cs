namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterUpdateRequest
    {
        public long? RequisitionId { get; set; }
        public string? FilterName { get; set; }
        public bool? IsAutoShortlistEnabled { get; set; }
        public bool? RunOnSubmission { get; set; }
        public bool? IsActive { get; set; }
    }
}
