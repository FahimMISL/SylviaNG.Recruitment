using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.QuestionGroups.Commands.QuestionGroupCreate
{
    public class QuestionGroupCreateValidator : AbstractValidator<QuestionGroupCreateCommand>
    {
        public QuestionGroupCreateValidator()
        {
            RuleFor(x => x.Request.Name)
                .NotEmpty().WithMessage("Question group name is required.")
                .MaximumLength(200).WithMessage("Question group name must not exceed 200 characters.");
        }
    }
}
