using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

/// <summary>
/// Admin-managed lookup for candidate gender options (dynamic dropdown, replaces the old
/// MaritalStatusEnum - HR/Admin can add/rename/delete values without a code deploy).
/// </summary>
public class MaritalStatus : Audit
{
    public long MaritalStatusId { get; set; }
    public string Name { get; set; } = string.Empty;
}
