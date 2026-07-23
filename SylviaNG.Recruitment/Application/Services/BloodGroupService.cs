using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.BloodGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class BloodGroupService : IBloodGroupService
    {
        private readonly IBloodGroupRepository _genderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BloodGroupService(IBloodGroupRepository genderRepository, IUnitOfWork unitOfWork)
        {
            _genderRepository = genderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(BloodGroupCreateRequest request)
        {
            var exists = await _genderRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("BloodGroup", "Name", request.Name);

            var entity = request.ToEntity();
            await _genderRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.BloodGroupId;
        }

        public async Task UpdateAsync(long bloodGroupId, BloodGroupUpdateRequest request)
        {
            var entity = await _genderRepository.GetByIdAsync(bloodGroupId)
                ?? throw new NotFoundException("BloodGroup", bloodGroupId);

            var nameTaken = await _genderRepository.ExistsByNameAsync(request.Name, bloodGroupId);
            if (nameTaken)
                throw new DuplicateException("BloodGroup", "Name", request.Name);

            entity.Name = request.Name;
            _genderRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long bloodGroupId)
        {
            var entity = await _genderRepository.GetByIdAsync(bloodGroupId)
                ?? throw new NotFoundException("BloodGroup", bloodGroupId);

            var usageCount = await _genderRepository.CountUsageAsync(bloodGroupId);
            if (usageCount > 0)
                throw new ResourceInUseException("BloodGroup", bloodGroupId, usageCount);

            _genderRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<BloodGroupResponse>> GetAllAsync()
        {
            var entities = await _genderRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
