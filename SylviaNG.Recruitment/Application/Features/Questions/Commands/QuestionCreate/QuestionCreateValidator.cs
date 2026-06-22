using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.Questions.Commands.QuestionCreate
{
    public class QuestionCreateValidator : AbstractValidator<QuestionCreateCommand>
    {
        public QuestionCreateValidator()
        {
            RuleFor(x => x.Request.QuestionGroupId).GreaterThan(0).WithMessage("QuestionGroupId is required.");
        }
    }
}
