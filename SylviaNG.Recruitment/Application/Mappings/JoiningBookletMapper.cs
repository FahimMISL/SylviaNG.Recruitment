using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class JoiningBookletMapper
    {
        public static JoiningBooklet ToEntity(this JoiningBookletCreateRequest request)
        {
            return new JoiningBooklet
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                FileUrl = request.FileUrl,
                FileName = request.FileName,
                GeneratedAt = request.GeneratedAt,
                GeneratedByUserId = request.GeneratedByUserId,
            };
        }

        public static void ApplyUpdate(this JoiningBooklet entity, JoiningBookletUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.FileUrl is not null) entity.FileUrl = request.FileUrl;
            if (request.FileName is not null) entity.FileName = request.FileName;
            if (request.GeneratedAt.HasValue) entity.GeneratedAt = request.GeneratedAt.Value;
            if (request.GeneratedByUserId.HasValue) entity.GeneratedByUserId = request.GeneratedByUserId.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static JoiningBookletResponse ToResponse(this JoiningBooklet entity)
        {
            return new JoiningBookletResponse
            {
                JoiningBookletId = entity.JoiningBookletId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                FileUrl = entity.FileUrl,
                FileName = entity.FileName,
                GeneratedAt = entity.GeneratedAt,
                GeneratedByUserId = entity.GeneratedByUserId,
                IsActive = entity.IsActive,
            };
        }
    }
}
