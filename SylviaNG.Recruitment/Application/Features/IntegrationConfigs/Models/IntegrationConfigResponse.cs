using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models
{
    public class IntegrationConfigResponse
    {
        public long IntegrationConfigId { get; set; }
        public IntegrationTypeEnum IntegrationType { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public string? BaseUrl { get; set; }
        public bool HasApiKey { get; set; }
        public string? AdditionalSettingsJson { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? LastTestedAt { get; set; }
        public bool? LastTestSuccess { get; set; }
        public bool IsActive { get; set; }
    }
}
