using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ShortlistFilterCriteria : Audit
{
    public long ShortlistFilterCriteriaId { get; set; }
    public long ShortlistFilterId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsHardFilter { get; set; } = true;
    public int LayerOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ShortlistFilter ShortlistFilter { get; set; } = null!;
}
