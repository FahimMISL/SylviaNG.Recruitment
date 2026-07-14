using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateEducationService : ICandidateEducationService
    {
        private readonly ICandidateEducationRepository _candidateEducationRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateEducationService(
            ICandidateEducationRepository candidateEducationRepository,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _candidateEducationRepository = candidateEducationRepository;
            _currentCandidateService = currentCandidateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CandidateEducationResponse>> GetAllForCurrentCandidateAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _candidateEducationRepository.GetAllByCandidateProfileIdAsync(profileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> CreateAsync(CandidateEducationCreateRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = request.ToEntity(profileId);

            await _candidateEducationRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateEducationId;
        }

        public async Task UpdateAsync(long candidateEducationId, CandidateEducationUpdateRequest request)
        {
            var entity = await GetOwnedEntityAsync(candidateEducationId);

            entity.ApplyUpdate(request);
            entity.UpdatedAt = DateTime.UtcNow;

            _candidateEducationRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateEducationId)
        {
            var entity = await GetOwnedEntityAsync(candidateEducationId);

            _candidateEducationRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Domain.Entities.CandidateEducation> GetOwnedEntityAsync(long candidateEducationId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateEducationRepository.GetByIdAsync(candidateEducationId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateEducation", candidateEducationId);

            return entity;
        }
    }
}
