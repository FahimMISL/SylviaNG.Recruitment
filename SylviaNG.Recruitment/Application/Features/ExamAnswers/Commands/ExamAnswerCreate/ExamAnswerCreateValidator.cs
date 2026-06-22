using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamAnswers.Commands.ExamAnswerCreate
{
    public class ExamAnswerCreateValidator : AbstractValidator<ExamAnswerCreateCommand>
    {
        public ExamAnswerCreateValidator()
        {
            RuleFor(x => x.Request.ExamCandidateId).GreaterThan(0).WithMessage("ExamCandidateId is required.");
        }
    }
}
