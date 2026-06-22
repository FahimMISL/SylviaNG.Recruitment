using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class QuestionGroupMapper
    {
        public static QuestionGroup ToEntity(this QuestionGroupCreateRequest request)
        {
            return new QuestionGroup
            {
                GroupName = request.GroupName,
                Category = request.Category,
                DifficultyLevel = request.DifficultyLevel,
            };
        }

        public static void ApplyUpdate(this QuestionGroup entity, QuestionGroupUpdateRequest request)
        {
            if (request.GroupName is not null) entity.GroupName = request.GroupName;
            if (request.Category is not null) entity.Category = request.Category;
            if (request.DifficultyLevel is not null) entity.DifficultyLevel = request.DifficultyLevel;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static QuestionGroupResponse ToResponse(this QuestionGroup entity)
        {
            return new QuestionGroupResponse
            {
                QuestionGroupId = entity.QuestionGroupId,
                GroupName = entity.GroupName,
                Category = entity.Category,
                DifficultyLevel = entity.DifficultyLevel,
                IsActive = entity.IsActive,
            };
        }
    }
}
