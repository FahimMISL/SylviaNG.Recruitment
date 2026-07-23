using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ScorecardService : IScorecardService
    {
        private readonly IScorecardRepository _scorecardRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ScorecardService(IScorecardRepository scorecardRepository, IUnitOfWork unitOfWork)
        {
            _scorecardRepository = scorecardRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ScorecardCreateRequest request)
        {
            var exists = await _scorecardRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Scorecard", "Name", request.Name);

            var entity = request.ToEntity();
            NormalizeDisplayOrder(entity.Criteria);

            await _scorecardRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ScorecardId;
        }

        public async Task UpdateAsync(long scorecardId, ScorecardUpdateRequest request)
        {
            var entity = await _scorecardRepository.GetByIdWithCriteriaAsync(scorecardId)
                ?? throw new NotFoundException("Scorecard", scorecardId);

            var nameTaken = await _scorecardRepository.ExistsByNameAsync(request.Name, scorecardId);
            if (nameTaken)
                throw new DuplicateException("Scorecard", "Name", request.Name);

            entity.Name = request.Name;
            entity.Description = request.Description;

            // Whole-collection replace: same precedent as ShortlistFilterService.UpdateAsync -
            // nothing else references ScorecardCriterionId at template-edit time (evaluations
            // reference criteria by id, but editing a template after evaluations exist is an
            // accepted edge case, same as editing a ShortlistFilter after it's been applied).
            entity.Criteria.Clear();
            var newCriteria = request.Criteria.OrderBy(c => c.DisplayOrder).Select(c => c.ToEntity()).ToList();
            NormalizeDisplayOrder(newCriteria);
            foreach (var criterion in newCriteria)
            {
                entity.Criteria.Add(criterion);
            }

            _scorecardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetActiveStatusAsync(long scorecardId, bool isActive)
        {
            var entity = await _scorecardRepository.GetByIdAsync(scorecardId)
                ?? throw new NotFoundException("Scorecard", scorecardId);

            entity.IsActive = isActive;
            _scorecardRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ScorecardResponse> GetByIdAsync(long scorecardId)
        {
            var entity = await _scorecardRepository.GetByIdWithCriteriaAsync(scorecardId)
                ?? throw new NotFoundException("Scorecard", scorecardId);

            return entity.ToResponse();
        }

        public async Task<List<ScorecardResponse>> GetAllAsync()
        {
            var entities = await _scorecardRepository.GetAllWithCriteriaAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<ScorecardLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _scorecardRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        private static void NormalizeDisplayOrder(IEnumerable<Domain.Entities.ScorecardCriterion> criteria)
        {
            var order = 0;
            foreach (var criterion in criteria)
            {
                criterion.DisplayOrder = order++;
            }
        }
    }
}
