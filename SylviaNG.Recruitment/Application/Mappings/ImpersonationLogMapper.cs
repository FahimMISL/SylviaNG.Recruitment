using SylviaNG.Recruitment.Application.Features.ImpersonationLogs.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ImpersonationLogMapper
    {
        public static ImpersonationLog ToEntity(this ImpersonationLogCreateRequest request)
        {
            return new ImpersonationLog
            {
                AdminUserId = request.AdminUserId,
                CandidateId = request.CandidateId,
                Reason = request.Reason,
                StartedAt = request.StartedAt
            };
        }

        public static void ApplyUpdate(this ImpersonationLog entity, ImpersonationLogUpdateRequest request)
        {
            if (request.Reason is not null) entity.Reason = request.Reason;
            if (request.EndedAt.HasValue) entity.EndedAt = request.EndedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ImpersonationLogResponse ToResponse(this ImpersonationLog entity)
        {
            return new ImpersonationLogResponse
            {
                ImpersonationLogId = entity.ImpersonationLogId,
                AdminUserId = entity.AdminUserId,
                CandidateId = entity.CandidateId,
                Reason = entity.Reason,
                StartedAt = entity.StartedAt,
                EndedAt = entity.EndedAt,
                IsActive = entity.IsActive
            };
        }
    }
}
