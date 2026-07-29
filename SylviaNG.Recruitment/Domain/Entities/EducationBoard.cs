using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup of examination boards (Dhaka Board, Madrasah Board, Bangladesh Open
/// University, Edexcel, etc). Shown in the candidate Education section only for degrees whose
/// Degree.Position marks them SSC/HSC-equivalent - see Degree.Position for the grouping.
/// </summary>
public class EducationBoard : Audit
{
    public long EducationBoardId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
