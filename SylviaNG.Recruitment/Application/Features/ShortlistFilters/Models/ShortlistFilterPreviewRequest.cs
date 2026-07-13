namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    /// <summary>
    /// Exactly one of ShortlistFilterId (preview a saved filter) or Definition (preview an
    /// unsaved draft) must be set - enforced by ShortlistFilterPreviewValidator (US-043 AC5).
    /// </summary>
    public class ShortlistFilterPreviewRequest
    {
        public long? ShortlistFilterId { get; set; }
        public ShortlistFilterDefinitionRequest? Definition { get; set; }
        public long JobPostingId { get; set; }
    }
}
