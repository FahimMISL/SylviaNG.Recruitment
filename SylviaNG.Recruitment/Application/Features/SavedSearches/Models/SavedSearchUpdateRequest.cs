namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Models
{
    public class SavedSearchUpdateRequest
    {
        public long? CreatedByUserId { get; set; }
        public string? SearchName { get; set; }
        public string? QueryExpression { get; set; }
        public bool? IsActive { get; set; }
    }
}
