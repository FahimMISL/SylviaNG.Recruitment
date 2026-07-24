using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for university-level Major Subject options (dynamic dropdown,
/// searchable in the UI, replaces the old CandidateEducation.MajorSubject free-text field
/// for Degree.Position 3+ entries).
/// </summary>
public class MajorSubjectUniversity : Audit
{
    public long MajorSubjectUniversityId { get; set; }
    public string Name { get; set; } = string.Empty;
}
