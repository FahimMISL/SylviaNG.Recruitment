using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.MedicalTests.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class MedicalTestService : IMedicalTestService
    {
        private readonly IMedicalTestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public MedicalTestService(IMedicalTestRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(MedicalTestCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.MedicalTestId;
        }

        public async Task UpdateAsync(long medicalTestId, MedicalTestUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(medicalTestId)
                ?? throw new KeyNotFoundException($"MedicalTest with ID {medicalTestId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long medicalTestId)
        {
            var entity = await _repository.GetByIdAsync(medicalTestId)
                ?? throw new KeyNotFoundException($"MedicalTest with ID {medicalTestId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<MedicalTestResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<MedicalTestResponse> GetByIdAsync(long medicalTestId)
        {
            var entity = await _repository.GetByIdAsync(medicalTestId)
                ?? throw new KeyNotFoundException($"MedicalTest with ID {medicalTestId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<MedicalTestResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<MedicalTestResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
