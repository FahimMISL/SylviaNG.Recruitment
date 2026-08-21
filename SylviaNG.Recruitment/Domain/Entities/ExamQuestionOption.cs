using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// One selectable option belonging to an MCQ or True/False ExamQuestion (US-053 AC3).
/// </summary>
public class ExamQuestionOption : Audit, ICompanyScoped
{
    public long ExamQuestionOptionId { get; set; }

    // Multi-tenant: mirrors the parent ExamQuestion's CompanyId, stamped at creation.
    public long? CompanyId { get; set; }
    public long ExamQuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public ExamQuestion ExamQuestion { get; set; } = null!;
}
