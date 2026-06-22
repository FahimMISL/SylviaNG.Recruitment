namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Models
{
    public class SavedSearchResponse
    {
        public long SavedSearchId { get; set; }
        public long CreatedByUserId { get; set; }
        public string SearchName { get; set; } = string.Empty;
        public string QueryExpression { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
