using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.SpecialCategories.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class SpecialCategoryService : ISpecialCategoryService
    {
        private readonly ISpecialCategoryRepository _specialCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialCategoryService(ISpecialCategoryRepository specialCategoryRepository, IUnitOfWork unitOfWork)
        {
            _specialCategoryRepository = specialCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SpecialCategoryCreateRequest request)
        {
            var exists = await _specialCategoryRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("SpecialCategory", "Name", request.Name);

            var entity = request.ToEntity();
            await _specialCategoryRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.SpecialCategoryId;
        }

        public async Task UpdateAsync(long specialCategoryId, SpecialCategoryUpdateRequest request)
        {
            var entity = await _specialCategoryRepository.GetByIdAsync(specialCategoryId)
                ?? throw new NotFoundException("SpecialCategory", specialCategoryId);

            var nameTaken = await _specialCategoryRepository.ExistsByNameAsync(request.Name, specialCategoryId);
            if (nameTaken)
                throw new DuplicateException("SpecialCategory", "Name", request.Name);

            entity.Name = request.Name;
            _specialCategoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long specialCategoryId)
        {
            var entity = await _specialCategoryRepository.GetByIdAsync(specialCategoryId)
                ?? throw new NotFoundException("SpecialCategory", specialCategoryId);

            var usageCount = await _specialCategoryRepository.CountUsageAsync(specialCategoryId);
            if (usageCount > 0)
                throw new ResourceInUseException("SpecialCategory", specialCategoryId, usageCount);

            _specialCategoryRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SpecialCategoryResponse>> GetAllAsync()
        {
            var entities = await _specialCategoryRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
