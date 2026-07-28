namespace SylviaNG.Recruitment.Application.Features.ExamTaking.Models
{
    public class ExamSubmitRequest
    {
        public List<ExamAnswerRequest> Answers { get; set; } = new();
    }

    public class ExamAnswerRequest
    {
        public long ExamQuestionId { get; set; }

        /// <summary>McqSingle/McqMultiple/TrueFalse - the ExamQuestionOptionId(s) the candidate selected.</summary>
        public List<long>? SelectedOptionIds { get; set; }

        /// <summary>Subjective only.</summary>
        public string? AnswerText { get; set; }
    }
}
