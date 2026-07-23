using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.Interviews.Commands.InterviewMarkResult
{
    public class InterviewMarkResultValidator : AbstractValidator<InterviewMarkResultCommand>
    {
        public InterviewMarkResultValidator()
        {
            RuleFor(x => x.Request.Result)
                .Must(r => r == InterviewResultEnum.Passed || r == InterviewResultEnum.Failed)
                .WithMessage("Result must be Passed or Failed.");
        }
    }
}
