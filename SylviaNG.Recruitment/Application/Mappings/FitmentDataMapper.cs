using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class FitmentDataMapper
    {
        public static FitmentData ToEntity(this FitmentDataCreateRequest request)
        {
            return new FitmentData
            {
                CandidateId = request.CandidateId,
                JobApplicationId = request.JobApplicationId,
                ProposedGrade = request.ProposedGrade,
                ProposedRole = request.ProposedRole,
                Location = request.Location,
                SalaryStructureJson = request.SalaryStructureJson,
                PayrollSource = request.PayrollSource,
                IsFetchedFromPayroll = request.IsFetchedFromPayroll,
                IsManualEntry = request.IsManualEntry,
                FetchedAt = request.FetchedAt,
            };
        }

        public static void ApplyUpdate(this FitmentData entity, FitmentDataUpdateRequest request)
        {
            if (request.CandidateId.HasValue) entity.CandidateId = request.CandidateId.Value;
            if (request.JobApplicationId.HasValue) entity.JobApplicationId = request.JobApplicationId.Value;
            if (request.ProposedGrade is not null) entity.ProposedGrade = request.ProposedGrade;
            if (request.ProposedRole is not null) entity.ProposedRole = request.ProposedRole;
            if (request.Location is not null) entity.Location = request.Location;
            if (request.SalaryStructureJson is not null) entity.SalaryStructureJson = request.SalaryStructureJson;
            if (request.PayrollSource is not null) entity.PayrollSource = request.PayrollSource;
            if (request.IsFetchedFromPayroll.HasValue) entity.IsFetchedFromPayroll = request.IsFetchedFromPayroll.Value;
            if (request.IsManualEntry.HasValue) entity.IsManualEntry = request.IsManualEntry.Value;
            if (request.FetchedAt.HasValue) entity.FetchedAt = request.FetchedAt.Value;
            if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        }

        public static FitmentDataResponse ToResponse(this FitmentData entity)
        {
            return new FitmentDataResponse
            {
                FitmentDataId = entity.FitmentDataId,
                CandidateId = entity.CandidateId,
                JobApplicationId = entity.JobApplicationId,
                ProposedGrade = entity.ProposedGrade,
                ProposedRole = entity.ProposedRole,
                Location = entity.Location,
                SalaryStructureJson = entity.SalaryStructureJson,
                PayrollSource = entity.PayrollSource,
                IsFetchedFromPayroll = entity.IsFetchedFromPayroll,
                IsManualEntry = entity.IsManualEntry,
                FetchedAt = entity.FetchedAt,
                IsActive = entity.IsActive,
            };
        }
    }
}
