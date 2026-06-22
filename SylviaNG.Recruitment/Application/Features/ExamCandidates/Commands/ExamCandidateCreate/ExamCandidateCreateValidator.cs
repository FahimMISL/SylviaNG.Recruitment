using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamCandidates.Commands.ExamCandidateCreate
{
    public class ExamCandidateCreateValidator : AbstractValidator<ExamCandidateCreateCommand>
    {
        public ExamCandidateCreateValidator()
        {
            RuleFor(x => x.Request.ExamId)
                .GreaterThan(0).WithMessage("ExamId is required.");
        }
    }
}
