using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ProfileFieldConfigs.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ProfileFieldConfigService : IProfileFieldConfigService
    {
        private readonly IProfileFieldConfigRepository _profileFieldConfigRepository;
        private readonly IJobPostingRepository _jobPostingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileFieldConfigService(
            IProfileFieldConfigRepository profileFieldConfigRepository,
            IJobPostingRepository jobPostingRepository,
            IUnitOfWork unitOfWork)
        {
            _profileFieldConfigRepository = profileFieldConfigRepository;
            _jobPostingRepository = jobPostingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ProfileFieldConfigRequest request)
        {
            await EnsureValidAsync(request);

            var exists = await _profileFieldConfigRepository.ExistsAsync(request.Field, request.JobPostingId);
            if (exists)
                throw new DuplicateException($"A config for field \"{request.Field}\" already exists for this scope.");

            var entity = new ProfileFieldConfig
            {
                Field = request.Field,
                JobPostingId = request.JobPostingId,
                Visibility = request.Visibility
            };

            await _profileFieldConfigRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ProfileFieldConfigId;
        }

        public async Task UpdateAsync(long profileFieldConfigId, ProfileFieldConfigRequest request)
        {
            var entity = await _profileFieldConfigRepository.GetByIdAsync(profileFieldConfigId)
                ?? throw new NotFoundException("ProfileFieldConfig", profileFieldConfigId);

            await EnsureValidAsync(request);

            var exists = await _profileFieldConfigRepository.ExistsAsync(request.Field, request.JobPostingId, profileFieldConfigId);
            if (exists)
                throw new DuplicateException($"A config for field \"{request.Field}\" already exists for this scope.");

            entity.Field = request.Field;
            entity.JobPostingId = request.JobPostingId;
            entity.Visibility = request.Visibility;

            _profileFieldConfigRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long profileFieldConfigId)
        {
            var entity = await _profileFieldConfigRepository.GetByIdAsync(profileFieldConfigId)
                ?? throw new NotFoundException("ProfileFieldConfig", profileFieldConfigId);

            _profileFieldConfigRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ProfileFieldConfigResponse>> GetAllAsync()
        {
            var entities = await _profileFieldConfigRepository.GetAllAsync();
            return entities.Select(e => new ProfileFieldConfigResponse
            {
                ProfileFieldConfigId = e.ProfileFieldConfigId,
                Field = e.Field,
                JobPostingId = e.JobPostingId,
                Visibility = e.Visibility
            }).ToList();
        }

        public async Task<List<EffectiveProfileFieldResponse>> GetEffectiveConfigAsync(long? jobPostingId)
        {
            var rows = await _profileFieldConfigRepository.GetGlobalAndForJobPostingAsync(jobPostingId);

            var allFields = Enum.GetValues<CandidateProfileFieldEnum>();
            var result = new List<EffectiveProfileFieldResponse>();

            foreach (var field in allFields)
            {
                var postingOverride = jobPostingId.HasValue
                    ? rows.FirstOrDefault(r => r.Field == field && r.JobPostingId == jobPostingId)
                    : null;
                var global = rows.FirstOrDefault(r => r.Field == field && r.JobPostingId == null);

                var visibility = postingOverride?.Visibility ?? global?.Visibility ?? ProfileFieldVisibilityEnum.Optional;

                result.Add(new EffectiveProfileFieldResponse { Field = field, Visibility = visibility });
            }

            return result;
        }

        private async Task EnsureValidAsync(ProfileFieldConfigRequest request)
        {
            if (request.JobPostingId.HasValue)
            {
                var jobPosting = await _jobPostingRepository.GetByIdAsync(request.JobPostingId.Value);
                if (jobPosting is null)
                    throw new NotFoundException("JobPosting", request.JobPostingId.Value);
            }
        }
    }
}
