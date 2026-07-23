using SylviaNG.Recruitment.Application.Features.ApplicationSettings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ApplicationSettingService : IApplicationSettingService
    {
        private readonly IApplicationSettingRepository _applicationSettingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationSettingService(IApplicationSettingRepository applicationSettingRepository, IUnitOfWork unitOfWork)
        {
            _applicationSettingRepository = applicationSettingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationSettingResponse> GetAsync()
        {
            var entity = await _applicationSettingRepository.GetSingletonAsync();
            return new ApplicationSettingResponse
            {
                MinimumProfileCompletenessPercentage = entity.MinimumProfileCompletenessPercentage
            };
        }

        public async Task UpdateAsync(ApplicationSettingUpdateRequest request)
        {
            if (request.MinimumProfileCompletenessPercentage is < 0 or > 100)
            {
                throw new FluentValidation.ValidationException(new[]
                {
                    new FluentValidation.Results.ValidationFailure(
                        nameof(request.MinimumProfileCompletenessPercentage),
                        "MinimumProfileCompletenessPercentage must be between 0 and 100.")
                });
            }

            var entity = await _applicationSettingRepository.GetSingletonAsync();
            entity.MinimumProfileCompletenessPercentage = request.MinimumProfileCompletenessPercentage;
            _applicationSettingRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetMinimumProfileCompletenessPercentageAsync()
        {
            var entity = await _applicationSettingRepository.GetSingletonAsync();
            return entity.MinimumProfileCompletenessPercentage;
        }
    }
}
