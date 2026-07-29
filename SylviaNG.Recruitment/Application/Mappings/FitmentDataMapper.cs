using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Application.Mappings
{
    public static class FitmentDataMapper
    {
        public static FitmentDataResponse ToResponse(this FitmentData entity)
        {
            return new FitmentDataResponse
            {
                FitmentDataId = entity.FitmentDataId,
                JobApplicationId = entity.JobApplicationId,
                Designation = entity.Designation,
                Grade = entity.Grade,
                Location = entity.Location,
                BasicSalary = entity.BasicSalary,
                TotalAllowances = entity.TotalAllowances,
                TotalDeductions = entity.TotalDeductions,
            };
        }
    }
}
