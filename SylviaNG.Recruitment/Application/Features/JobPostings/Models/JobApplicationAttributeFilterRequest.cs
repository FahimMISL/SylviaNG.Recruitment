using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.JobPostings.Models
{
    /// <summary>
    /// ATS dashboard filter params (US-035 scalar filters + US-050 candidate-attribute filters).
    /// The candidate-attribute fields (everything below Status/Source/DateFrom/DateTo) require
    /// JobPostingId to be set - enforced by JobApplicationAttributeFilterValidator.
    /// </summary>
    public class JobApplicationAttributeFilterRequest
    {
        public long? JobPostingId { get; set; }
        public ApplicationStatusEnum? Status { get; set; }
        public ApplicationSourceEnum? Source { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public EducationLevelEnum? MinEducationLevel { get; set; }
        public decimal? MinExperienceYears { get; set; }
        public decimal? MaxExperienceYears { get; set; }
        public List<string>? Skills { get; set; }
        public string? Location { get; set; }
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public bool HasCandidateAttributeFilters =>
            MinEducationLevel.HasValue
            || MinExperienceYears.HasValue
            || MaxExperienceYears.HasValue
            || (Skills != null && Skills.Count > 0)
            || !string.IsNullOrWhiteSpace(Location)
            || MinAge.HasValue
            || MaxAge.HasValue;
    }
}
