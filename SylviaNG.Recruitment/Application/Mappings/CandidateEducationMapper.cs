using SylviaNG.Recruitment.Application.Features.CandidateEducations.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class CandidateEducationMapper
    {
        public static CandidateEducation ToEntity(this CandidateEducationCreateRequest request)
        {
            return new CandidateEducation
            {
                CandidateId = request.CandidateId,
                Degree = request.Degree,
                FieldOfStudy = request.FieldOfStudy,
                Institution = request.Institution,
                Board = request.Board,
                PassingYear = request.PassingYear,
                Result = request.Result,
                ResultScale = request.ResultScale,
            };
        }

        public static void ApplyUpdate(this CandidateEducation entity, CandidateEducationUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.Degree is not null) entity.Degree = request.Degree;
            if (request.FieldOfStudy is not null) entity.FieldOfStudy = request.FieldOfStudy;
            if (request.Institution is not null) entity.Institution = request.Institution;
            if (request.Board is not null) entity.Board = request.Board;
            if (request.PassingYear.HasValue) entity.PassingYear = request.PassingYear.Value;
            if (request.Result is not null) entity.Result = request.Result;
            if (request.ResultScale is not null) entity.ResultScale = request.ResultScale;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static CandidateEducationResponse ToResponse(this CandidateEducation entity)
        {
            return new CandidateEducationResponse
            {
                CandidateEducationId = entity.CandidateEducationId,
                CandidateId = entity.CandidateId,
                Degree = entity.Degree,
                FieldOfStudy = entity.FieldOfStudy,
                Institution = entity.Institution,
                Board = entity.Board,
                PassingYear = entity.PassingYear,
                Result = entity.Result,
                ResultScale = entity.ResultScale,
                IsActive = entity.IsActive,
            };
        }
    }
}
