using FluentValidation;

namespace SylviaNG.Recruitment.Application.Features.QuestionOptions.Commands.QuestionOptionCreate
{
    public class QuestionOptionCreateValidator : AbstractValidator<QuestionOptionCreateCommand>
    {
        public QuestionOptionCreateValidator()
        {
            RuleFor(x => x.Request.QuestionId).GreaterThan(0).WithMessage("QuestionId is required.");
        }
    }
}
