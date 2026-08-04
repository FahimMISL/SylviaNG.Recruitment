using FluentValidation;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionCreate
{
    public class ExamQuestionCreateValidator : AbstractValidator<ExamQuestionCreateCommand>
    {
        public ExamQuestionCreateValidator()
        {
            RuleFor(x => x.Request.QuestionGroupId)
                .GreaterThan(0).WithMessage("QuestionGroupId is required.");

            RuleFor(x => x.Request.QuestionText)
                .NotEmpty().WithMessage("Question text is required.");

            RuleFor(x => x.Request.Marks)
                .GreaterThan(0).WithMessage("Marks must be greater than 0.");

            RuleFor(x => x.Request.Options)
                .Empty().WithMessage("Subjective questions must not have any options.")
                .When(x => x.Request.QuestionType == QuestionTypeEnum.Subjective);

            RuleFor(x => x.Request.Options)
                .Must(options => options.Count == 2
                    && options.Any(o => o.OptionText.Equals("True", StringComparison.OrdinalIgnoreCase))
                    && options.Any(o => o.OptionText.Equals("False", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("True/False questions must have exactly 2 options named 'True' and 'False'.")
                .When(x => x.Request.QuestionType == QuestionTypeEnum.TrueFalse);

            RuleFor(x => x.Request.Options)
                .Must(options => options.Count >= 2)
                .WithMessage("MCQ questions need at least 2 options.")
                .When(x => x.Request.QuestionType is QuestionTypeEnum.McqSingle or QuestionTypeEnum.McqMultiple);

            RuleFor(x => x.Request.Options)
                .Must(options => options.Count(o => o.IsCorrect) == 1)
                .WithMessage("MCQ (single correct) and True/False questions must have exactly 1 correct option.")
                .When(x => x.Request.QuestionType is QuestionTypeEnum.McqSingle or QuestionTypeEnum.TrueFalse);

            RuleFor(x => x.Request.Options)
                .Must(options => options.Any(o => o.IsCorrect))
                .WithMessage("MCQ (multiple correct) questions must have at least 1 correct option.")
                .When(x => x.Request.QuestionType == QuestionTypeEnum.McqMultiple);

            RuleForEach(x => x.Request.Options).ChildRules(option =>
            {
                option.RuleFor(o => o.OptionText)
                    .NotEmpty().WithMessage("Option text is required.");
            });
        }
    }
}
