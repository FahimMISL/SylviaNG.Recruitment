using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ProfileFieldConfigMapper
    {
        public static ProfileFieldConfig ToEntity(this ProfileFieldConfigCreateRequest request)
        {
            return new ProfileFieldConfig
            {
                FieldName = request.FieldName,
                Label = request.Label,
                FieldType = request.FieldType,
                IsRequired = request.IsRequired,
                IsVisible = request.IsVisible,
                SortOrder = request.SortOrder,
                OptionsJson = request.OptionsJson,
            };
        }

        public static void ApplyUpdate(this ProfileFieldConfig entity, ProfileFieldConfigUpdateRequest request)
        {
            if (request.Label is not null) entity.Label = request.Label;
            if (request.FieldType is not null) entity.FieldType = request.FieldType;
            if (request.IsRequired.HasValue) entity.IsRequired = request.IsRequired.Value;
            if (request.IsVisible.HasValue) entity.IsVisible = request.IsVisible.Value;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.OptionsJson is not null) entity.OptionsJson = request.OptionsJson;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ProfileFieldConfigResponse ToResponse(this ProfileFieldConfig entity)
        {
            return new ProfileFieldConfigResponse
            {
                ProfileFieldConfigId = entity.ProfileFieldConfigId,
                FieldName = entity.FieldName,
                Label = entity.Label,
                FieldType = entity.FieldType,
                IsRequired = entity.IsRequired,
                IsVisible = entity.IsVisible,
                SortOrder = entity.SortOrder,
                OptionsJson = entity.OptionsJson,
                IsActive = entity.IsActive,
            };
        }
    }
}
