using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class SavedReportMapper
    {
        public static SavedReport ToEntity(this SavedReportCreateRequest request)
        {
            return new SavedReport
            {
                CreatedByUserId = request.CreatedByUserId,
                ReportName = request.ReportName,
                Description = request.Description,
                ReportType = request.ReportType,
                FilterCriteriaJson = request.FilterCriteriaJson,
                ColumnConfigJson = request.ColumnConfigJson,
            };
        }

        public static void ApplyUpdate(this SavedReport entity, SavedReportUpdateRequest request)
        {
            if (request.CreatedByUserId.HasValue) entity.CreatedByUserId = request.CreatedByUserId.Value;
            if (request.ReportName is not null) entity.ReportName = request.ReportName;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.ReportType is not null) entity.ReportType = request.ReportType;
            if (request.FilterCriteriaJson is not null) entity.FilterCriteriaJson = request.FilterCriteriaJson;
            if (request.ColumnConfigJson is not null) entity.ColumnConfigJson = request.ColumnConfigJson;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static SavedReportResponse ToResponse(this SavedReport entity)
        {
            return new SavedReportResponse
            {
                SavedReportId = entity.SavedReportId,
                CreatedByUserId = entity.CreatedByUserId,
                ReportName = entity.ReportName,
                Description = entity.Description,
                ReportType = entity.ReportType,
                FilterCriteriaJson = entity.FilterCriteriaJson,
                ColumnConfigJson = entity.ColumnConfigJson,
                IsActive = entity.IsActive,
            };
        }
    }
}
