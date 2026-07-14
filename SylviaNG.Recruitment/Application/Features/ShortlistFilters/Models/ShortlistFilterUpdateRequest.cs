using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public FilterCombinatorEnum CombineWith { get; set; } = FilterCombinatorEnum.And;

        /// <summary>Full replacement of the filter's criteria list (add/edit/remove/reorder in one save).</summary>
        public List<ShortlistFilterCriterionRequest> Criteria { get; set; } = new();
    }
}
