using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for QuestionGroup. No AutoMapper, matching ShortlistFilterMapper.
    /// </summary>
    public static class QuestionGroupMapper
    {
        public static QuestionGroup ToEntity(this QuestionGroupCreateRequest request)
        {
            return new QuestionGroup
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };
        }

        public static QuestionGroupResponse ToResponse(this QuestionGroup entity)
        {
            return new QuestionGroupResponse
            {
                QuestionGroupId = entity.QuestionGroupId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }

        public static QuestionGroupLookupResponse ToLookupResponse(this QuestionGroup entity)
        {
            return new QuestionGroupLookupResponse
            {
                QuestionGroupId = entity.QuestionGroupId,
                Name = entity.Name
            };
        }
    }
}
