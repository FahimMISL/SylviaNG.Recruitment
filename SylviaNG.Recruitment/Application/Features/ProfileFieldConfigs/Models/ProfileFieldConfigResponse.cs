using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models
{
    public class ProfileFieldConfigResponse
    {
        public long ProfileFieldConfigId { get; set; }
        public CandidateProfileFieldEnum Field { get; set; }
        public long? JobPostingId { get; set; }
        public ProfileFieldVisibilityEnum Visibility { get; set; }
    }

    /// <summary>One field's merged, effective visibility for a given apply-form context
    /// (global default, overridden by a job-posting-specific row when one exists).</summary>
    public class EffectiveProfileFieldResponse
    {
        public CandidateProfileFieldEnum Field { get; set; }
        public ProfileFieldVisibilityEnum Visibility { get; set; }
    }
}
