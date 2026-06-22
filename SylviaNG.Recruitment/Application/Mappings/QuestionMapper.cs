using SylviaNG.Recruitment.Application.Features.Questions.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class QuestionMapper
    {
        public static Question ToEntity(this QuestionCreateRequest request)
        {
            return new Question
            {
                QuestionGroupId = request.QuestionGroupId,
                QuestionType = request.QuestionType,
                QuestionText = request.QuestionText,
                Marks = request.Marks,
                TimeLimitSeconds = request.TimeLimitSeconds,
                Explanation = request.Explanation,
            };
        }

        public static void ApplyUpdate(this Question entity, QuestionUpdateRequest request)
        {
            if (request.QuestionGroupId.HasValue) entity.QuestionGroupId = request.QuestionGroupId.Value;
            if (request.QuestionType.HasValue) entity.QuestionType = request.QuestionType.Value;
            if (request.QuestionText is not null) entity.QuestionText = request.QuestionText;
            if (request.Marks.HasValue) entity.Marks = request.Marks.Value;
            if (request.TimeLimitSeconds.HasValue) entity.TimeLimitSeconds = request.TimeLimitSeconds.Value;
            if (request.Explanation is not null) entity.Explanation = request.Explanation;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static QuestionResponse ToResponse(this Question entity)
        {
            return new QuestionResponse
            {
                QuestionId = entity.QuestionId,
                QuestionGroupId = entity.QuestionGroupId,
                QuestionType = entity.QuestionType,
                QuestionText = entity.QuestionText,
                Marks = entity.Marks,
                TimeLimitSeconds = entity.TimeLimitSeconds,
                Explanation = entity.Explanation,
                IsActive = entity.IsActive,
            };
        }
    }
}
