using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.CandidateProfiles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class UniversityLibraryService : IUniversityLibraryService
    {
        private readonly IUniversityLibraryItemRepository _universityLibraryItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UniversityLibraryService(IUniversityLibraryItemRepository universityLibraryItemRepository, IUnitOfWork unitOfWork)
        {
            _universityLibraryItemRepository = universityLibraryItemRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UniversityLibraryItemResponse>> GetAllAsync()
        {
            var entities = await _universityLibraryItemRepository.GetAllOrderedAsync();
            return entities.Select(e => new UniversityLibraryItemResponse
            {
                UniversityLibraryItemId = e.UniversityLibraryItemId,
                Name = e.Name,
                Code = e.Code
            }).ToList();
        }

        public async Task<long> CreateAsync(UniversityLibraryItemCreateRequest request)
        {
            var exists = await _universityLibraryItemRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("University", "Name", request.Name);

            var entity = new UniversityLibraryItem
            {
                Name = request.Name,
                Code = request.Code,
            };

            await _universityLibraryItemRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.UniversityLibraryItemId;
        }

        public async Task UpdateAsync(long universityLibraryItemId, UniversityLibraryItemUpdateRequest request)
        {
            var entity = await _universityLibraryItemRepository.GetByIdAsync(universityLibraryItemId)
                ?? throw new NotFoundException("University", universityLibraryItemId);

            var nameTaken = await _universityLibraryItemRepository.ExistsByNameAsync(request.Name, universityLibraryItemId);
            if (nameTaken)
                throw new DuplicateException("University", "Name", request.Name);

            entity.Name = request.Name;
            entity.Code = request.Code;
            _universityLibraryItemRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long universityLibraryItemId)
        {
            var entity = await _universityLibraryItemRepository.GetByIdAsync(universityLibraryItemId)
                ?? throw new NotFoundException("University", universityLibraryItemId);

            var usageCount = await _universityLibraryItemRepository.CountUsageAsync(universityLibraryItemId);
            if (usageCount > 0)
                throw new ResourceInUseException("University", universityLibraryItemId, usageCount);

            _universityLibraryItemRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
