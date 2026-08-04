using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Genders.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class GenderService : IGenderService
    {
        private readonly IGenderRepository _genderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GenderService(IGenderRepository genderRepository, IUnitOfWork unitOfWork)
        {
            _genderRepository = genderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(GenderCreateRequest request)
        {
            var exists = await _genderRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Gender", "Name", request.Name);

            var entity = request.ToEntity();
            await _genderRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.GenderId;
        }

        public async Task UpdateAsync(long genderId, GenderUpdateRequest request)
        {
            var entity = await _genderRepository.GetByIdAsync(genderId)
                ?? throw new NotFoundException("Gender", genderId);

            var nameTaken = await _genderRepository.ExistsByNameAsync(request.Name, genderId);
            if (nameTaken)
                throw new DuplicateException("Gender", "Name", request.Name);

            entity.Name = request.Name;
            _genderRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long genderId)
        {
            var entity = await _genderRepository.GetByIdAsync(genderId)
                ?? throw new NotFoundException("Gender", genderId);

            var usageCount = await _genderRepository.CountUsageAsync(genderId);
            if (usageCount > 0)
                throw new ResourceInUseException("Gender", genderId, usageCount);

            _genderRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<GenderResponse>> GetAllAsync()
        {
            var entities = await _genderRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
