using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class ExamCandidateMapper
    {
        public static ExamCandidate ToEntity(this ExamCandidateCreateRequest request)
        {
            return new ExamCandidate
            {
                ExamId = request.ExamId,
                JobApplicationId = request.JobApplicationId,
                StartedAt = request.StartedAt,
                SubmittedAt = request.SubmittedAt,
                ObtainedMarks = request.ObtainedMarks,
                IsPassed = request.IsPassed,
                IsAutoGraded = request.IsAutoGraded,
            };
        }

        public static void ApplyUpdate(this ExamCandidate entity, ExamCandidateUpdateRequest request)
        {
            if (request.ExamId.HasValue) entity.ExamId = request.ExamId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.StartedAt.HasValue) entity.StartedAt = request.StartedAt;
            if (request.SubmittedAt.HasValue) entity.SubmittedAt = request.SubmittedAt;
            if (request.ObtainedMarks.HasValue) entity.ObtainedMarks = request.ObtainedMarks.Value;
            if (request.IsPassed.HasValue) entity.IsPassed = request.IsPassed.Value;
            if (request.IsAutoGraded.HasValue) entity.IsAutoGraded = request.IsAutoGraded.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static ExamCandidateResponse ToResponse(this ExamCandidate entity)
        {
            return new ExamCandidateResponse
            {
                ExamCandidateId = entity.ExamCandidateId,
                ExamId = entity.ExamId,
                JobApplicationId = entity.JobApplicationId,
                StartedAt = entity.StartedAt,
                SubmittedAt = entity.SubmittedAt,
                ObtainedMarks = entity.ObtainedMarks,
                IsPassed = entity.IsPassed,
                IsAutoGraded = entity.IsAutoGraded,
                IsActive = entity.IsActive,
            };
        }
    }
}
