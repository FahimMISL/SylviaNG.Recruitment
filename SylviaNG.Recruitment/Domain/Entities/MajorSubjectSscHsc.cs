using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for SSC/HSC-level Major Subject options (dynamic dropdown, replaces
/// the old CandidateEducation.MajorSubject free-text field for Degree.Position 1/2 entries).
/// </summary>
public class MajorSubjectSscHsc : Audit
{
    public long MajorSubjectSscHscId { get; set; }
    public string Name { get; set; } = string.Empty;
}
