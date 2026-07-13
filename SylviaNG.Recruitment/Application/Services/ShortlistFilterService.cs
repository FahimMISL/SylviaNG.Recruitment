using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ShortlistFilterService : IShortlistFilterService
    {
        private readonly IShortlistFilterRepository _shortlistFilterRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ShortlistFilterService(IShortlistFilterRepository shortlistFilterRepository, IUnitOfWork unitOfWork)
        {
            _shortlistFilterRepository = shortlistFilterRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ShortlistFilterCreateRequest request)
        {
            var exists = await _shortlistFilterRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("ShortlistFilter", "Name", request.Name);

            var entity = request.ToEntity();
            NormalizeDisplayOrder(entity.Criteria);

            await _shortlistFilterRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ShortlistFilterId;
        }

        public async Task UpdateAsync(long shortlistFilterId, ShortlistFilterUpdateRequest request)
        {
            var entity = await _shortlistFilterRepository.GetByIdWithCriteriaAsync(shortlistFilterId)
                ?? throw new NotFoundException("ShortlistFilter", shortlistFilterId);

            var nameTaken = await _shortlistFilterRepository.ExistsByNameAsync(request.Name, shortlistFilterId);
            if (nameTaken)
                throw new DuplicateException("ShortlistFilter", "Name", request.Name);

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.CombineWith = request.CombineWith;

            // Whole-collection replace: nothing else references ShortlistFilterCriterionId yet,
            // same precedent as HiringPipelineService.UpdateAsync's stage replacement.
            entity.Criteria.Clear();
            var newCriteria = request.Criteria.OrderBy(c => c.DisplayOrder).Select(c => c.ToEntity()).ToList();
            NormalizeDisplayOrder(newCriteria);
            foreach (var criterion in newCriteria)
            {
                entity.Criteria.Add(criterion);
            }

            _shortlistFilterRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long shortlistFilterId)
        {
            var entity = await _shortlistFilterRepository.GetByIdAsync(shortlistFilterId)
                ?? throw new NotFoundException("ShortlistFilter", shortlistFilterId);

            _shortlistFilterRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ShortlistFilterResponse> GetByIdAsync(long shortlistFilterId)
        {
            var entity = await _shortlistFilterRepository.GetByIdWithCriteriaAsync(shortlistFilterId)
                ?? throw new NotFoundException("ShortlistFilter", shortlistFilterId);

            return entity.ToResponse();
        }

        public async Task<List<ShortlistFilterResponse>> GetAllAsync()
        {
            var entities = await _shortlistFilterRepository.GetAllWithCriteriaAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<List<ShortlistFilterLookupResponse>> GetActiveLookupAsync()
        {
            var entities = await _shortlistFilterRepository.GetActiveAsync();
            return entities.Select(e => e.ToLookupResponse()).ToList();
        }

        private static void NormalizeDisplayOrder(IEnumerable<Domain.Entities.ShortlistFilterCriterion> criteria)
        {
            var order = 0;
            foreach (var criterion in criteria)
            {
                criterion.DisplayOrder = order++;
            }
        }
    }
}
