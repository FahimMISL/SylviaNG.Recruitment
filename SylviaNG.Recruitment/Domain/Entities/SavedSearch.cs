using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class SavedSearch : Audit
{
    public long SavedSearchId { get; set; }
    public long CreatedByUserId { get; set; }
    public string SearchName { get; set; } = string.Empty;
    public string QueryExpression { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
}
