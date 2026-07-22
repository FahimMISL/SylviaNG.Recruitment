using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateTagService : ICandidateTagService
    {
        private const int SuggestionLimit = 20;

        private readonly ICandidateTagRepository _candidateTagRepository;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateTagService(
            ICandidateTagRepository candidateTagRepository,
            ICandidateProfileRepository candidateProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _candidateTagRepository = candidateTagRepository;
            _candidateProfileRepository = candidateProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CandidateTagResponse>> GetAllAsync(long candidateProfileId)
        {
            var entities = await _candidateTagRepository.GetAllByCandidateProfileIdAsync(candidateProfileId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> CreateAsync(long candidateProfileId, CandidateTagCreateRequest request)
        {
            await EnsureProfileExistsAsync(candidateProfileId);

            var tagName = request.TagName.Trim();

            var existing = await _candidateTagRepository.GetAllByCandidateProfileIdAsync(candidateProfileId);
            if (existing.Any(t => string.Equals(t.TagName, tagName, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateException("CandidateTag", "TagName", tagName);

            var entity = new CandidateTag
            {
                CandidateProfileId = candidateProfileId,
                TagName = tagName
            };

            await _candidateTagRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CandidateTagId;
        }

        public async Task DeleteAsync(long candidateProfileId, long candidateTagId)
        {
            var entity = await _candidateTagRepository.GetByIdAsync(candidateTagId);

            if (entity == null || entity.CandidateProfileId != candidateProfileId)
                throw new NotFoundException("CandidateTag", candidateTagId);

            _candidateTagRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<string>> GetSuggestionsAsync(string? search)
        {
            return await _candidateTagRepository.GetDistinctTagNamesAsync(search, SuggestionLimit);
        }

        private async Task EnsureProfileExistsAsync(long candidateProfileId)
        {
            var profile = await _candidateProfileRepository.GetByIdAsync(candidateProfileId);
            if (profile == null)
                throw new NotFoundException("CandidateProfile", candidateProfileId);
        }
    }
}
