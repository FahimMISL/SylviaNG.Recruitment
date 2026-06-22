using SylviaNG.Recruitment.Application.Features.IntegrationConfigs.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class IntegrationConfigMapper
    {
        public static IntegrationConfig ToEntity(this IntegrationConfigCreateRequest request)
        {
            return new IntegrationConfig
            {
                IntegrationType = request.IntegrationType,
                ConfigName = request.ConfigName,
                BaseUrl = request.BaseUrl,
                ApiKeyEncrypted = request.ApiKeyEncrypted,
                AdditionalSettingsJson = request.AdditionalSettingsJson,
                IsEnabled = request.IsEnabled,
                LastTestedAt = request.LastTestedAt,
                LastTestSuccess = request.LastTestSuccess,
            };
        }

        public static void ApplyUpdate(this IntegrationConfig entity, IntegrationConfigUpdateRequest request)
        {
            if (request.IntegrationType.HasValue) entity.IntegrationType = request.IntegrationType.Value;
            if (request.ConfigName is not null) entity.ConfigName = request.ConfigName;
            if (request.BaseUrl is not null) entity.BaseUrl = request.BaseUrl;
            if (request.ApiKeyEncrypted is not null) entity.ApiKeyEncrypted = request.ApiKeyEncrypted;
            if (request.AdditionalSettingsJson is not null) entity.AdditionalSettingsJson = request.AdditionalSettingsJson;
            if (request.IsEnabled.HasValue) entity.IsEnabled = request.IsEnabled.Value;
            if (request.LastTestedAt.HasValue) entity.LastTestedAt = request.LastTestedAt;
            if (request.LastTestSuccess.HasValue) entity.LastTestSuccess = request.LastTestSuccess.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static IntegrationConfigResponse ToResponse(this IntegrationConfig entity)
        {
            return new IntegrationConfigResponse
            {
                IntegrationConfigId = entity.IntegrationConfigId,
                IntegrationType = entity.IntegrationType,
                ConfigName = entity.ConfigName,
                BaseUrl = entity.BaseUrl,
                HasApiKey = !string.IsNullOrWhiteSpace(entity.ApiKeyEncrypted),
                AdditionalSettingsJson = entity.AdditionalSettingsJson,
                IsEnabled = entity.IsEnabled,
                LastTestedAt = entity.LastTestedAt,
                LastTestSuccess = entity.LastTestSuccess,
                IsActive = entity.IsActive,
            };
        }
    }
}
