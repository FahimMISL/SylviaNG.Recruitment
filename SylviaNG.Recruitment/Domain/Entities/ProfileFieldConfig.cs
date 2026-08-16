using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// EP-15/US-116: configures whether a candidate profile field is Mandatory/Optional/Hidden.
/// JobPostingId null = global default; a non-null row overrides the global default for that one
/// posting's apply form. Uniqueness of (Field, JobPostingId) is enforced in
/// ProfileFieldConfigService, not a DB constraint (Postgres unique indexes don't treat multiple
/// NULLs as a conflict, which is exactly the "one global row per field" case here).
/// </summary>
public class ProfileFieldConfig : Audit
{
    public long ProfileFieldConfigId { get; set; }
    public CandidateProfileFieldEnum Field { get; set; }
    public long? JobPostingId { get; set; }
    public ProfileFieldVisibilityEnum Visibility { get; set; }

    public JobPosting? JobPosting { get; set; }
}
