using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateWorkExperienceService : ICandidateWorkExperienceService
    {
        private readonly ICandidateWorkExperienceRepository _candidateWorkExperienceRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateWorkExperienceService(
            ICandidateWorkExperienceRepository candidateWorkExperienceRepository,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _candidateWorkExperienceRepository = candidateWorkExperienceRepository;
            _currentCandidateService = currentCandidateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CandidateWorkExperienceResponse>> GetAllForCurrentCandidateAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _candidateWorkExperienceRepository.GetAllByCandidateProfileIdAsync(profileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> CreateAsync(CandidateWorkExperienceCreateRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = request.ToEntity(profileId);

            await _candidateWorkExperienceRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateWorkExperienceId;
        }

        public async Task UpdateAsync(long candidateWorkExperienceId, CandidateWorkExperienceUpdateRequest request)
        {
            var entity = await GetOwnedEntityAsync(candidateWorkExperienceId);

            entity.ApplyUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateWorkExperienceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateWorkExperienceId)
        {
            var entity = await GetOwnedEntityAsync(candidateWorkExperienceId);

            _candidateWorkExperienceRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Domain.Entities.CandidateWorkExperience> GetOwnedEntityAsync(long candidateWorkExperienceId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateWorkExperienceRepository.GetByIdAsync(candidateWorkExperienceId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateWorkExperience", candidateWorkExperienceId);

            return entity;
        }
    }
}
