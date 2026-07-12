using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateSkillService : ICandidateSkillService
    {
        private readonly ICandidateSkillRepository _candidateSkillRepository;
        private readonly ICurrentCandidateService _currentCandidateService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateSkillService(
            ICandidateSkillRepository candidateSkillRepository,
            ICurrentCandidateService currentCandidateService,
            IUnitOfWork unitOfWork)
        {
            _candidateSkillRepository = candidateSkillRepository;
            _currentCandidateService = currentCandidateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CandidateSkillResponse>> GetAllForCurrentCandidateAsync()
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entities = await _candidateSkillRepository.GetAllByCandidateProfileIdAsync(profileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> CreateAsync(CandidateSkillCreateRequest request)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = request.ToEntity(profileId);

            await _candidateSkillRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateSkillId;
        }

        public async Task DeleteAsync(long candidateSkillId)
        {
            var profileId = await _currentCandidateService.GetOrCreateCurrentProfileIdAsync();
            var entity = await _candidateSkillRepository.GetByIdAsync(candidateSkillId);

            if (entity == null || entity.CandidateProfileId != profileId)
                throw new NotFoundException("CandidateSkill", candidateSkillId);

            _candidateSkillRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
