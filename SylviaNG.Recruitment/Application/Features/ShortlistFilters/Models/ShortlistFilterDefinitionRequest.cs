using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    /// <summary>An unsaved filter definition, for previewing edits before they're saved (US-043 AC5).</summary>
    public class ShortlistFilterDefinitionRequest
    {
        public FilterCombinatorEnum CombineWith { get; set; } = FilterCombinatorEnum.And;
        public List<ShortlistFilterCriterionRequest> Criteria { get; set; } = new();
    }
}
