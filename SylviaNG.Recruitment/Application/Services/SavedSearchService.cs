using FluentValidation;
using FluentValidation.Results;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class SavedSearchService : ISavedSearchService
    {
        private readonly ISavedSearchRepository _savedSearchRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public SavedSearchService(
            ISavedSearchRepository savedSearchRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _savedSearchRepository = savedSearchRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SavedSearchCreateRequest request)
        {
            var ownerUserName = _currentUserService.GetCurrentUserName();
            if (string.IsNullOrWhiteSpace(ownerUserName))
                throw new ValidationException(new[]
                {
                    new ValidationFailure(nameof(ownerUserName), "Could not resolve the current user - a saved search must have a definite owner.")
                });

            ValidateRequest(request.Name, request.FilterJson);

            var exists = await _savedSearchRepository.ExistsByNameForOwnerAsync(ownerUserName, request.Name);
            if (exists)
                throw new DuplicateException("SavedSearch", "Name", request.Name);

            var entity = request.ToEntity(ownerUserName);

            await _savedSearchRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.SavedSearchId;
        }

        public async Task UpdateAsync(long savedSearchId, SavedSearchUpdateRequest request)
        {
            var entity = await _savedSearchRepository.GetByIdAsync(savedSearchId)
                ?? throw new NotFoundException("SavedSearch", savedSearchId);

            EnsureOwnerOrAdmin(entity.OwnerUserName);

            ValidateRequest(request.Name, request.FilterJson);

            var nameTaken = await _savedSearchRepository.ExistsByNameForOwnerAsync(entity.OwnerUserName, request.Name, savedSearchId);
            if (nameTaken)
                throw new DuplicateException("SavedSearch", "Name", request.Name);

            entity.Name = request.Name;
            entity.IsShared = request.IsShared;
            entity.FilterJson = request.FilterJson;

            _savedSearchRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long savedSearchId)
        {
            var entity = await _savedSearchRepository.GetByIdAsync(savedSearchId)
                ?? throw new NotFoundException("SavedSearch", savedSearchId);

            EnsureOwnerOrAdmin(entity.OwnerUserName);

            _savedSearchRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SavedSearchLookupResponse>> GetVisibleLookupAsync()
        {
            var currentUserName = _currentUserService.GetCurrentUserName();
            var entities = await _savedSearchRepository.GetVisibleAsync(currentUserName ?? string.Empty);

            return entities.Select(e => e.ToLookupResponse(currentUserName)).ToList();
        }

        private void EnsureOwnerOrAdmin(string ownerUserName)
        {
            var currentUserName = _currentUserService.GetCurrentUserName();
            if (ownerUserName != currentUserName && !_currentUserService.IsInRole("Admin"))
                throw new ForbiddenException("You do not have permission to modify this saved search.");
        }

        private static void ValidateRequest(string name, string filterJson)
        {
            var failures = new List<ValidationFailure>();

            if (string.IsNullOrWhiteSpace(name))
                failures.Add(new ValidationFailure(nameof(name), "Name is required."));
            else if (name.Length > 200)
                failures.Add(new ValidationFailure(nameof(name), "Name must be 200 characters or fewer."));

            if (string.IsNullOrWhiteSpace(filterJson))
                failures.Add(new ValidationFailure(nameof(filterJson), "FilterJson is required."));

            if (failures.Count > 0)
                throw new ValidationException(failures);
        }
    }
}
