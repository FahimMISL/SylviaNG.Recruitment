using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    /// <summary>
    /// Manual mapping methods for ShortlistFilter and ShortlistFilterCriterion entities.
    /// </summary>
    public static class ShortlistFilterMapper
    {
        public static ShortlistFilterCriterion ToEntity(this ShortlistFilterCriterionRequest request)
        {
            return new ShortlistFilterCriterion
            {
                CriterionType = request.CriterionType,
                DisplayOrder = request.DisplayOrder,
                MinEducationLevel = request.MinEducationLevel,
                MinExperienceYears = request.MinExperienceYears,
                RequiredSkillNames = request.RequiredSkillNames,
                MinAge = request.MinAge,
                MaxAge = request.MaxAge,
                RequiredDistrict = request.RequiredDistrict,
                MinScreeningScore = request.MinScreeningScore
            };
        }

        /// <summary>Converts a saved criterion row back into the request shape, so the evaluation
        /// engine can treat saved and unsaved (preview-draft) criteria identically.</summary>
        public static ShortlistFilterCriterionRequest ToCriterionRequest(this ShortlistFilterCriterion entity)
        {
            return new ShortlistFilterCriterionRequest
            {
                CriterionType = entity.CriterionType,
                DisplayOrder = entity.DisplayOrder,
                MinEducationLevel = entity.MinEducationLevel,
                MinExperienceYears = entity.MinExperienceYears,
                RequiredSkillNames = entity.RequiredSkillNames,
                MinAge = entity.MinAge,
                MaxAge = entity.MaxAge,
                RequiredDistrict = entity.RequiredDistrict,
                MinScreeningScore = entity.MinScreeningScore
            };
        }

        public static ShortlistFilterCriterionResponse ToResponse(this ShortlistFilterCriterion entity)
        {
            return new ShortlistFilterCriterionResponse
            {
                ShortlistFilterCriterionId = entity.ShortlistFilterCriterionId,
                CriterionType = entity.CriterionType,
                DisplayOrder = entity.DisplayOrder,
                MinEducationLevel = entity.MinEducationLevel,
                MinExperienceYears = entity.MinExperienceYears,
                RequiredSkillNames = entity.RequiredSkillNames,
                MinAge = entity.MinAge,
                MaxAge = entity.MaxAge,
                RequiredDistrict = entity.RequiredDistrict,
                MinScreeningScore = entity.MinScreeningScore
            };
        }

        public static ShortlistFilter ToEntity(this ShortlistFilterCreateRequest request)
        {
            return new ShortlistFilter
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CombineWith = request.CombineWith,
                Criteria = request.Criteria.OrderBy(c => c.DisplayOrder).Select(c => c.ToEntity()).ToList()
            };
        }

        public static ShortlistFilterResponse ToResponse(this ShortlistFilter entity)
        {
            return new ShortlistFilterResponse
            {
                ShortlistFilterId = entity.ShortlistFilterId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CombineWith = entity.CombineWith,
                Criteria = entity.Criteria?
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => c.ToResponse())
                    .ToList() ?? new List<ShortlistFilterCriterionResponse>()
            };
        }

        public static ShortlistFilterLookupResponse ToLookupResponse(this ShortlistFilter entity)
        {
            return new ShortlistFilterLookupResponse
            {
                ShortlistFilterId = entity.ShortlistFilterId,
                Name = entity.Name
            };
        }
    }
}
