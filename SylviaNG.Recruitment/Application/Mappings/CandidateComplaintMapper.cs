using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateComplaintMapper
    {
        public static CandidateComplaint ToEntity(this CandidateComplaintCreateRequest request)
        {
            return new CandidateComplaint
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                Category = request.Category,
                Description = request.Description,
                AssignedToUserId = request.AssignedToUserId
            };
        }

        public static void ApplyUpdate(this CandidateComplaint entity, CandidateComplaintUpdateRequest request)
        {
            if (request.Category is not null) entity.Category = request.Category;
            if (request.Description is not null) entity.Description = request.Description;
            if (request.ComplaintStatus.HasValue) entity.ComplaintStatus = request.ComplaintStatus.Value;
            if (request.AssignedToUserId.HasValue) entity.AssignedToUserId = request.AssignedToUserId;
            if (request.ResolutionNotes is not null) entity.ResolutionNotes = request.ResolutionNotes;
            if (request.ResolvedAt.HasValue) entity.ResolvedAt = request.ResolvedAt;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateComplaintResponse ToResponse(this CandidateComplaint entity)
        {
            return new CandidateComplaintResponse
            {
                CandidateComplaintId = entity.CandidateComplaintId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                Category = entity.Category,
                Description = entity.Description,
                ComplaintStatus = entity.ComplaintStatus,
                AssignedToUserId = entity.AssignedToUserId,
                ResolutionNotes = entity.ResolutionNotes,
                ResolvedAt = entity.ResolvedAt,
                IsActive = entity.IsActive
            };
        }
    }
}
