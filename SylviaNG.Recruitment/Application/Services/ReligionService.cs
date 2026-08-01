using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Religions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ReligionService : IReligionService
    {
        private readonly IReligionRepository _genderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReligionService(IReligionRepository genderRepository, IUnitOfWork unitOfWork)
        {
            _genderRepository = genderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ReligionCreateRequest request)
        {
            var exists = await _genderRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Religion", "Name", request.Name);

            var entity = request.ToEntity();
            await _genderRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReligionId;
        }

        public async Task UpdateAsync(long religionId, ReligionUpdateRequest request)
        {
            var entity = await _genderRepository.GetByIdAsync(religionId)
                ?? throw new NotFoundException("Religion", religionId);

            var nameTaken = await _genderRepository.ExistsByNameAsync(request.Name, religionId);
            if (nameTaken)
                throw new DuplicateException("Religion", "Name", request.Name);

            entity.Name = request.Name;
            _genderRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long religionId)
        {
            var entity = await _genderRepository.GetByIdAsync(religionId)
                ?? throw new NotFoundException("Religion", religionId);

            var usageCount = await _genderRepository.CountUsageAsync(religionId);
            if (usageCount > 0)
                throw new ResourceInUseException("Religion", religionId, usageCount);

            _genderRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ReligionResponse>> GetAllAsync()
        {
            var entities = await _genderRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
