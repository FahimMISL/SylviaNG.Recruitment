using SylviaNG.Recruitment.Application.Features.DashboardWidgets.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class DashboardWidgetMapper
    {
        public static DashboardWidget ToEntity(this DashboardWidgetCreateRequest request)
        {
            return new DashboardWidget
            {
                UserId = request.UserId,
                WidgetType = request.WidgetType,
                Title = request.Title,
                ConfigJson = request.ConfigJson,
                SortOrder = request.SortOrder,
                IsVisible = request.IsVisible,
            };
        }

        public static void ApplyUpdate(this DashboardWidget entity, DashboardWidgetUpdateRequest request)
        {
            if (request.UserId.HasValue) entity.UserId = request.UserId.Value;
            if (request.WidgetType is not null) entity.WidgetType = request.WidgetType;
            if (request.Title is not null) entity.Title = request.Title;
            if (request.ConfigJson is not null) entity.ConfigJson = request.ConfigJson;
            if (request.SortOrder.HasValue) entity.SortOrder = request.SortOrder.Value;
            if (request.IsVisible.HasValue) entity.IsVisible = request.IsVisible.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static DashboardWidgetResponse ToResponse(this DashboardWidget entity)
        {
            return new DashboardWidgetResponse
            {
                DashboardWidgetId = entity.DashboardWidgetId,
                UserId = entity.UserId,
                WidgetType = entity.WidgetType,
                Title = entity.Title,
                ConfigJson = entity.ConfigJson,
                SortOrder = entity.SortOrder,
                IsVisible = entity.IsVisible,
                IsActive = entity.IsActive,
            };
        }
    }
}
