using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ExamQuestion and ExamQuestionOption. No AutoMapper, matching
    /// ShortlistFilterMapper. Deliberately flat - no QuestionType branching here; per-type shape
    /// rules (option count/correctness, True/False text) live in the FluentValidation validators.
    /// </summary>
    public static class ExamQuestionMapper
    {
        public static ExamQuestionOption ToEntity(this ExamQuestionOptionRequest request)
        {
            return new ExamQuestionOption
            {
                OptionText = request.OptionText,
                IsCorrect = request.IsCorrect,
                DisplayOrder = request.DisplayOrder
            };
        }

        public static ExamQuestionOptionResponse ToResponse(this ExamQuestionOption entity)
        {
            return new ExamQuestionOptionResponse
            {
                ExamQuestionOptionId = entity.ExamQuestionOptionId,
                OptionText = entity.OptionText,
                IsCorrect = entity.IsCorrect,
                DisplayOrder = entity.DisplayOrder
            };
        }

        public static ExamQuestion ToEntity(this ExamQuestionCreateRequest request)
        {
            return new ExamQuestion
            {
                QuestionGroupId = request.QuestionGroupId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType,
                DifficultyLevel = request.DifficultyLevel,
                Marks = request.Marks,
                Explanation = request.Explanation,
                ModelAnswer = request.ModelAnswer,
                IsActive = true,
                Options = request.Options.OrderBy(o => o.DisplayOrder).Select(o => o.ToEntity()).ToList()
            };
        }

        public static ExamQuestionResponse ToResponse(this ExamQuestion entity)
        {
            return new ExamQuestionResponse
            {
                ExamQuestionId = entity.ExamQuestionId,
                QuestionGroupId = entity.QuestionGroupId,
                QuestionGroupName = entity.QuestionGroup?.Name ?? string.Empty,
                QuestionText = entity.QuestionText,
                QuestionType = entity.QuestionType,
                DifficultyLevel = entity.DifficultyLevel,
                Marks = entity.Marks,
                Explanation = entity.Explanation,
                ModelAnswer = entity.ModelAnswer,
                IsActive = entity.IsActive,
                Options = entity.Options?
                    .OrderBy(o => o.DisplayOrder)
                    .Select(o => o.ToResponse())
                    .ToList() ?? new List<ExamQuestionOptionResponse>()
            };
        }
    }
}
