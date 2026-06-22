using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Audit;

namespace SylviaNG.Recruitment.Domain.Entities;

public class IntegrationConfig : Audit
{
    public long IntegrationConfigId { get; set; }
    public IntegrationTypeEnum IntegrationType { get; set; }
    public string ConfigName { get; set; } = string.Empty;
    public string? BaseUrl { get; set; }
    public string? ApiKeyEncrypted { get; set; }
    public string? AdditionalSettingsJson { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime? LastTestedAt { get; set; }
    public bool? LastTestSuccess { get; set; }
    public bool IsActive { get; set; } = true;
}
