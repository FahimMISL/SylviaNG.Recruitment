using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterResponse
    {
        public long ShortlistFilterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public FilterCombinatorEnum CombineWith { get; set; }
        public List<ShortlistFilterCriterionResponse> Criteria { get; set; } = new();
    }
}
