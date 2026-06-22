using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class ShortlistFilter : Audit
{
    public long ShortlistFilterId { get; set; }
    public long RequisitionId { get; set; }
    public string FilterName { get; set; } = string.Empty;
    public bool IsAutoShortlistEnabled { get; set; }
    public bool RunOnSubmission { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Requisition Requisition { get; set; } = null!;
    public ICollection<ShortlistFilterCriteria> Criteria { get; set; } = new List<ShortlistFilterCriteria>();
}
