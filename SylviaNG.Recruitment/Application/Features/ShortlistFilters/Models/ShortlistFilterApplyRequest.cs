namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    /// <summary>Apply a saved shortlist filter to a vacancy's applications in one action (US-044 AC1).
    /// Unlike Preview, only a saved filter can be applied - no unsaved Definition mode.</summary>
    public class ShortlistFilterApplyRequest
    {
        public long ShortlistFilterId { get; set; }
        public long JobPostingId { get; set; }
    }
}
