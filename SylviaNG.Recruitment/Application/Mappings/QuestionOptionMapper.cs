using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class QuestionOptionMapper
    {
        public static QuestionOption ToEntity(this QuestionOptionCreateRequest request)
        {
            return new QuestionOption
            {
                QuestionId = request.QuestionId,
                OptionText = request.OptionText,
                IsCorrect = request.IsCorrect,
                SortOrder = request.SortOrder,
            };
        }

        public static void ApplyUpdate(this QuestionOption entity, QuestionOptionUpdateRequest request)
        {
            if (request.QuestionId.HasValue) entity.QuestionId = request.QuestionId.Value;
            if (request.OptionText is not null) entity.OptionText = request.OptionText;
            if (request.IsCorrect.HasValue) entity.IsCorrect = request.IsCorrect.Value;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static QuestionOptionResponse ToResponse(this QuestionOption entity)
        {
            return new QuestionOptionResponse
            {
                QuestionOptionId = entity.QuestionOptionId,
                QuestionId = entity.QuestionId,
                OptionText = entity.OptionText,
                IsCorrect = entity.IsCorrect,
                SortOrder = entity.SortOrder,
                IsActive = entity.IsActive,
            };
        }
    }
}
