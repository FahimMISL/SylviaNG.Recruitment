using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models
{
    public class ShortlistFilterCriterionResponse
    {
        public long ShortlistFilterCriterionId { get; set; }
        public CriterionTypeEnum CriterionType { get; set; }
        public int DisplayOrder { get; set; }
        public EducationLevelEnum? MinEducationLevel { get; set; }
        public int? MinExperienceYears { get; set; }
        public string? RequiredSkillNames { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? RequiredDistrict { get; set; }
        public int? MinScreeningScore { get; set; }
    }
}
