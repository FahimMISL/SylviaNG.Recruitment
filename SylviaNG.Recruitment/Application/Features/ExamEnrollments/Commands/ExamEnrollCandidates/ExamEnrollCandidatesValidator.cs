using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.ExamEnrollments.Commands.ExamEnrollCandidates
{
    public class ExamEnrollCandidatesValidator : AbstractValidator<ExamEnrollCandidatesCommand>
    {
        public ExamEnrollCandidatesValidator()
        {
            RuleFor(x => x.ExamId)
                .GreaterThan(0).WithMessage("ExamId is required.");

            RuleFor(x => x.JobApplicationIds)
                .NotEmpty().WithMessage("Select at least one candidate to enroll.");
        }
    }
}
