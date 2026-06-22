using SylviaNG.Recruitment.Application.Features.Requisitions.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RequisitionMapper
    {
        public static Requisition ToEntity(this RequisitionCreateRequest request)
        {
            return new Requisition
            {
                SiteId = request.SiteId,
                DepartmentId = request.DepartmentId,
                DesignationId = request.DesignationId,
                Title = request.Title,
                JobDescription = request.JobDescription,
                Justification = request.Justification,
                RequisitionType = request.RequisitionType,
                RequisitionStatus = request.RequisitionStatus,
                NumberOfPositions = request.NumberOfPositions,
                BudgetCode = request.BudgetCode,
                RoleCategory = request.RoleCategory,
                ReplacementEmployeeId = request.ReplacementEmployeeId,
                ReplacementEmployeeName = request.ReplacementEmployeeName,
                ReplacementLastWorkingDate = request.ReplacementLastWorkingDate,
                RequestedByUserId = request.RequestedByUserId,
                ApprovedAt = request.ApprovedAt,
            };
        }

        public static void ApplyUpdate(this Requisition entity, RequisitionUpdateRequest request)
        {
            if (request.SiteId.HasValue) entity.SiteId = request.SiteId.Value;
            if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId.Value;
            if (request.DesignationId.HasValue) entity.DesignationId = request.DesignationId.Value;
            if (request.Title is not null) entity.Title = request.Title;
            if (request.JobDescription is not null) entity.JobDescription = request.JobDescription;
            if (request.Justification is not null) entity.Justification = request.Justification;
            if (request.RequisitionType.HasValue) entity.RequisitionType = request.RequisitionType.Value;
            if (request.RequisitionStatus.HasValue)
            {
                entity.RequisitionStatus = request.RequisitionStatus.Value;
                if (request.RequisitionStatus.Value == RequisitionStatusEnum.Approved && !entity.ApprovedAt.HasValue)
                    entity.ApprovedAt = DateTime.UtcNow;
            }
            if (request.NumberOfPositions.HasValue) entity.NumberOfPositions = request.NumberOfPositions.Value;
            if (request.BudgetCode is not null) entity.BudgetCode = request.BudgetCode;
            if (request.RoleCategory is not null) entity.RoleCategory = request.RoleCategory;
            if (request.ReplacementEmployeeId.HasValue) entity.ReplacementEmployeeId = request.ReplacementEmployeeId.Value;
            if (request.ReplacementEmployeeName is not null) entity.ReplacementEmployeeName = request.ReplacementEmployeeName;
            if (request.ReplacementLastWorkingDate.HasValue) entity.ReplacementLastWorkingDate = request.ReplacementLastWorkingDate;
            if (request.RequestedByUserId.HasValue) entity.RequestedByUserId = request.RequestedByUserId.Value;
            if (request.ApprovedAt.HasValue) entity.ApprovedAt = request.ApprovedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RequisitionResponse ToResponse(this Requisition entity)
        {
            return new RequisitionResponse
            {
                RequisitionId = entity.RequisitionId,
                SiteId = entity.SiteId,
                DepartmentId = entity.DepartmentId,
                DesignationId = entity.DesignationId,
                Title = entity.Title,
                JobDescription = entity.JobDescription,
                Justification = entity.Justification,
                RequisitionType = entity.RequisitionType,
                RequisitionStatus = entity.RequisitionStatus,
                NumberOfPositions = entity.NumberOfPositions,
                BudgetCode = entity.BudgetCode,
                RoleCategory = entity.RoleCategory,
                ReplacementEmployeeId = entity.ReplacementEmployeeId,
                ReplacementEmployeeName = entity.ReplacementEmployeeName,
                ReplacementLastWorkingDate = entity.ReplacementLastWorkingDate,
                RequestedByUserId = entity.RequestedByUserId,
                ApprovedAt = entity.ApprovedAt,
                CreatedAt = entity.CreatedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
