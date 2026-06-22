using SylviaNG.Recruitment.Application.Features.RequisitionApprovals.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class RequisitionApprovalMapper
    {
        public static RequisitionApproval ToEntity(this RequisitionApprovalCreateRequest request)
        {
            return new RequisitionApproval
            {
                RequisitionId = request.RequisitionId,
                ApproverUserId = request.ApproverUserId,
                ApproverRole = request.ApproverRole,
                ApprovalLevel = request.ApprovalLevel,
                Action = request.Action,
                Comments = request.Comments,
                ActionDate = request.ActionDate,
                IsPending = request.IsPending,
            };
        }

        public static void ApplyUpdate(this RequisitionApproval entity, RequisitionApprovalUpdateRequest request)
        {
            if (request.RequisitionId.HasValue) entity.RequisitionId = request.RequisitionId.Value;
            if (request.ApproverUserId.HasValue) entity.ApproverUserId = request.ApproverUserId.Value;
            if (request.ApproverRole is not null) entity.ApproverRole = request.ApproverRole;
            if (request.ApprovalLevel.HasValue) entity.ApprovalLevel = request.ApprovalLevel.Value;
            if (request.Action.HasValue) entity.Action = request.Action.Value;
            if (request.Comments is not null) entity.Comments = request.Comments;
            if (request.ActionDate.HasValue) entity.ActionDate = request.ActionDate;
            if (request.IsPending.HasValue) entity.IsPending = request.IsPending.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static RequisitionApprovalResponse ToResponse(this RequisitionApproval entity)
        {
            return new RequisitionApprovalResponse
            {
                RequisitionApprovalId = entity.RequisitionApprovalId,
                RequisitionId = entity.RequisitionId,
                ApproverUserId = entity.ApproverUserId,
                ApproverRole = entity.ApproverRole,
                ApprovalLevel = entity.ApprovalLevel,
                Action = entity.Action,
                Comments = entity.Comments,
                ActionDate = entity.ActionDate,
                IsPending = entity.IsPending,
                IsActive = entity.IsActive,
            };
        }
    }
}
