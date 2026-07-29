namespace SylviaNG.Recruitment.Application.Features.SavedSearches.Models
{
    public class SavedSearchCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool IsShared { get; set; }
        public string FilterJson { get; set; } = string.Empty;
    }
}
