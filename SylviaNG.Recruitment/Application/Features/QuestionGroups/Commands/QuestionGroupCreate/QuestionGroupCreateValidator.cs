using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate
{
    public class QuestionGroupCreateValidator : AbstractValidator<QuestionGroupCreateCommand>
    {
        public QuestionGroupCreateValidator()
        {
            RuleFor(x => x.Request.GroupName)
                .NotEmpty().WithMessage("GroupName is required.")
                .MaximumLength(500).WithMessage("GroupName must not exceed 500 characters.");
        }
    }
}
