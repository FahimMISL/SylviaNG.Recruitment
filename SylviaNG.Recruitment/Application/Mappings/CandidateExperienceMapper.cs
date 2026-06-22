using SylviaNG.Recruitment.Application.Features.CandidateExperiences.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateExperienceMapper
    {
        public static CandidateExperience ToEntity(this CandidateExperienceCreateRequest request)
        {
            return new CandidateExperience
            {
                CandidateId = request.CandidateId,
                OrganizationName = request.OrganizationName,
                Designation = request.Designation,
                Department = request.Department,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsCurrentJob = request.IsCurrentJob,
                Responsibilities = request.Responsibilities,
            };
        }

        public static void ApplyUpdate(this CandidateExperience entity, CandidateExperienceUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.OrganizationName is not null) entity.OrganizationName = request.OrganizationName;
            if (request.Designation is not null) entity.Designation = request.Designation;
            if (request.Department is not null) entity.Department = request.Department;
            if (request.StartDate.HasValue) entity.StartDate = request.StartDate;
            if (request.EndDate.HasValue) entity.EndDate = request.EndDate;
            if (request.IsCurrentJob.HasValue) entity.IsCurrentJob = request.IsCurrentJob.Value;
            if (request.Responsibilities is not null) entity.Responsibilities = request.Responsibilities;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateExperienceResponse ToResponse(this CandidateExperience entity)
        {
            return new CandidateExperienceResponse
            {
                CandidateExperienceId = entity.CandidateExperienceId,
                CandidateId = entity.CandidateId,
                OrganizationName = entity.OrganizationName,
                Designation = entity.Designation,
                Department = entity.Department,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsCurrentJob = entity.IsCurrentJob,
                Responsibilities = entity.Responsibilities,
                IsActive = entity.IsActive,
            };
        }
    }
}
