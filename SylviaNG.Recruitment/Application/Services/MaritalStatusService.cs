using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.MaritalStatuses.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class MaritalStatusService : IMaritalStatusService
    {
        private readonly IMaritalStatusRepository _genderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MaritalStatusService(IMaritalStatusRepository genderRepository, IUnitOfWork unitOfWork)
        {
            _genderRepository = genderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(MaritalStatusCreateRequest request)
        {
            var exists = await _genderRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("MaritalStatus", "Name", request.Name);

            var entity = request.ToEntity();
            await _genderRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.MaritalStatusId;
        }

        public async Task UpdateAsync(long maritalStatusId, MaritalStatusUpdateRequest request)
        {
            var entity = await _genderRepository.GetByIdAsync(maritalStatusId)
                ?? throw new NotFoundException("MaritalStatus", maritalStatusId);

            var nameTaken = await _genderRepository.ExistsByNameAsync(request.Name, maritalStatusId);
            if (nameTaken)
                throw new DuplicateException("MaritalStatus", "Name", request.Name);

            entity.Name = request.Name;
            _genderRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long maritalStatusId)
        {
            var entity = await _genderRepository.GetByIdAsync(maritalStatusId)
                ?? throw new NotFoundException("MaritalStatus", maritalStatusId);

            var usageCount = await _genderRepository.CountUsageAsync(maritalStatusId);
            if (usageCount > 0)
                throw new ResourceInUseException("MaritalStatus", maritalStatusId, usageCount);

            _genderRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MaritalStatusResponse>> GetAllAsync()
        {
            var entities = await _genderRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
