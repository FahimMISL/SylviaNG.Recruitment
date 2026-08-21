namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>Join entity: the question groups an online Exam draws its paper from (US-055) - an
/// exam can combine multiple groups (e.g. a general-aptitude pool plus a role-specific technical
/// pool). Same shape as InterviewPanelMember.</summary>
public class ExamQuestionGroup
{
    public long ExamId { get; set; }
    public long QuestionGroupId { get; set; }

    // Navigation properties
    public Exam Exam { get; set; } = null!;
    public QuestionGroup QuestionGroup { get; set; } = null!;
}
