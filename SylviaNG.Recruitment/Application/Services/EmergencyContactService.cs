using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.EmergencyContacts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class EmergencyContactService : IEmergencyContactService
    {
        private readonly IEmergencyContactRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public EmergencyContactService(IEmergencyContactRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(EmergencyContactCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.EmergencyContactId;
        }

        public async Task UpdateAsync(long emergencyContactId, EmergencyContactUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(emergencyContactId)
                ?? throw new KeyNotFoundException($"EmergencyContact with ID {emergencyContactId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long emergencyContactId)
        {
            var entity = await _repository.GetByIdAsync(emergencyContactId)
                ?? throw new KeyNotFoundException($"EmergencyContact with ID {emergencyContactId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EmergencyContactResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<EmergencyContactResponse> GetByIdAsync(long emergencyContactId)
        {
            var entity = await _repository.GetByIdAsync(emergencyContactId)
                ?? throw new KeyNotFoundException($"EmergencyContact with ID {emergencyContactId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<EmergencyContactResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<EmergencyContactResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
