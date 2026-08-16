using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupUpdate
{
    public class QuestionGroupUpdateValidator : AbstractValidator<QuestionGroupUpdateCommand>
    {
        public QuestionGroupUpdateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Question group name is required.")
                .MaximumLength(200).WithMessage("Question group name must not exceed 200 characters.");
        }
    }
}
