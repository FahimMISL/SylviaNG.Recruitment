using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FilterCombinatorEnum CombineWith { get; set; } = FilterCombinatorEnum.And;
        public List<ShortlistFilterCriterionRequest> Criteria { get; set; } = new();
    }
}
