namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Models
{
    public class QuestionGroupLookupResponse
    {
        public long QuestionGroupId { get; set; }
        public string Name { get; set; } = string.Empty;

        // Active question count - lets HR see a group is empty before picking it for an exam
        // (see IExamQuestionRepository.CountActiveByQuestionGroupIdsAsync's own doc comment).
        public int ActiveQuestionCount { get; set; }
    }
}
