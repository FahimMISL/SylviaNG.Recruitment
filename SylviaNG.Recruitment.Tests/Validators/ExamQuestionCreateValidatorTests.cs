using FluentAssertions;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Commands.ExamQuestionCreate;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Tests.Validators;

public class ExamQuestionCreateValidatorTests
{
    private readonly ExamQuestionCreateValidator _validator = new();

    private static ExamQuestionCreateRequest ValidMcqSingleRequest() => new()
    {
        QuestionGroupId = 1,
        QuestionText = "What is 2 + 2?",
        QuestionType = QuestionTypeEnum.McqSingle,
        DifficultyLevel = DifficultyLevelEnum.Easy,
        Marks = 5,
        Options = new List<ExamQuestionOptionRequest>
        {
            new() { OptionText = "3", IsCorrect = false, DisplayOrder = 0 },
            new() { OptionText = "4", IsCorrect = true, DisplayOrder = 1 }
        }
    };

    [Fact]
    public void Validate_WithValidMcqSingleRequest_ShouldHaveNoErrors()
    {
        var command = new ExamQuestionCreateCommand(ValidMcqSingleRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_McqSingleWithTwoCorrectOptions_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.Options[0].IsCorrect = true;
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Options");
    }

    [Fact]
    public void Validate_McqMultipleWithNoCorrectOptions_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.QuestionType = QuestionTypeEnum.McqMultiple;
        request.Options.ForEach(o => o.IsCorrect = false);
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("at least 1 correct option"));
    }

    [Fact]
    public void Validate_TrueFalseWithWrongOptionTexts_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.QuestionType = QuestionTypeEnum.TrueFalse;
        request.Options = new List<ExamQuestionOptionRequest>
        {
            new() { OptionText = "Yes", IsCorrect = true, DisplayOrder = 0 },
            new() { OptionText = "No", IsCorrect = false, DisplayOrder = 1 }
        };
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("'True' and 'False'"));
    }

    [Fact]
    public void Validate_SubjectiveWithOptionsPresent_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.QuestionType = QuestionTypeEnum.Subjective;
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("must not have any options"));
    }

    [Fact]
    public void Validate_MarksZeroOrNegative_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.Marks = 0;
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Marks");
    }

    [Fact]
    public void Validate_MissingQuestionGroupId_ShouldHaveValidationError()
    {
        var request = ValidMcqSingleRequest();
        request.QuestionGroupId = 0;
        var command = new ExamQuestionCreateCommand(request);

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.QuestionGroupId");
    }
}
