using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.FitmentDatas.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    // EP-12 US-097: manual-entry-only fitment data (grade/designation/location/salary structure) per
    // JobApplication - no Payroll auto-fetch, EP-16 System Integrations is out of scope for this
    // project. Standalone CRUD screen this round - OfferLetterService.GenerateAsync still takes
    // Designation/OfferedSalary as HR-typed request fields, pre-populating that form from
    // FitmentData is a deferred follow-on, not built here.
    public class FitmentDataService : IFitmentDataService
    {
        private readonly IFitmentDataRepository _fitmentDataRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FitmentDataService(
            IFitmentDataRepository fitmentDataRepository,
            IJobApplicationRepository jobApplicationRepository,
            IUnitOfWork unitOfWork)
        {
            _fitmentDataRepository = fitmentDataRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<FitmentDataResponse?> GetByJobApplicationIdAsync(long jobApplicationId)
        {
            var entity = await _fitmentDataRepository.GetByJobApplicationIdAsync(jobApplicationId);
            return entity?.ToResponse();
        }

        public async Task<FitmentDataResponse> UpsertAsync(FitmentDataUpsertRequest request)
        {
            _ = await _jobApplicationRepository.GetByIdAsync(request.JobApplicationId)
                ?? throw new NotFoundException("JobApplication", request.JobApplicationId);

            var entity = await _fitmentDataRepository.GetByJobApplicationIdAsync(request.JobApplicationId);
            if (entity == null)
            {
                entity = new Domain.Entities.FitmentData { JobApplicationId = request.JobApplicationId };
                await _fitmentDataRepository.AddAsync(entity);
            }

            entity.Designation = request.Designation;
            entity.Grade = request.Grade;
            entity.Location = request.Location;
            entity.BasicSalary = request.BasicSalary;
            entity.TotalAllowances = request.TotalAllowances;
            entity.TotalDeductions = request.TotalDeductions;

            await _unitOfWork.SaveChangesAsync();
            return entity.ToResponse();
        }
    }
}
