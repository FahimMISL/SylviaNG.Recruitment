using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class MedicalTestMapper
    {
        public static MedicalTest ToEntity(this MedicalTestCreateRequest request)
        {
            return new MedicalTest
            {
                VerificationWorkflowId = request.VerificationWorkflowId,
                FitnessStatus = request.FitnessStatus,
                MedicalCenter = request.MedicalCenter,
                TestDate = request.TestDate,
                ResultFileUrl = request.ResultFileUrl,
                Notes = request.Notes,
            };
        }

        public static void ApplyUpdate(this MedicalTest entity, MedicalTestUpdateRequest request)
        {
            if (request.VerificationWorkflowId.HasValue) entity.VerificationWorkflowId = request.VerificationWorkflowId.Value;
            if (request.FitnessStatus.HasValue) entity.FitnessStatus = request.FitnessStatus.Value;
            if (request.MedicalCenter is not null) entity.MedicalCenter = request.MedicalCenter;
            if (request.TestDate.HasValue) entity.TestDate = request.TestDate;
            if (request.ResultFileUrl is not null) entity.ResultFileUrl = request.ResultFileUrl;
            if (request.Notes is not null) entity.Notes = request.Notes;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static MedicalTestResponse ToResponse(this MedicalTest entity)
        {
            return new MedicalTestResponse
            {
                MedicalTestId = entity.MedicalTestId,
                VerificationWorkflowId = entity.VerificationWorkflowId,
                FitnessStatus = entity.FitnessStatus,
                MedicalCenter = entity.MedicalCenter,
                TestDate = entity.TestDate,
                ResultFileUrl = entity.ResultFileUrl,
                Notes = entity.Notes,
                IsActive = entity.IsActive,
            };
        }
    }
}
