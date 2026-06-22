using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ApplicationDuplicates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ApplicationDuplicateService : IApplicationDuplicateService
    {
        private readonly IApplicationDuplicateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationDuplicateService(IApplicationDuplicateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ApplicationDuplicateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ApplicationDuplicateId;
        }

        public async Task UpdateAsync(long applicationDuplicateId, ApplicationDuplicateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(applicationDuplicateId)
                ?? throw new KeyNotFoundException($"ApplicationDuplicate with ID {applicationDuplicateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long applicationDuplicateId)
        {
            var entity = await _repository.GetByIdAsync(applicationDuplicateId)
                ?? throw new KeyNotFoundException($"ApplicationDuplicate with ID {applicationDuplicateId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ApplicationDuplicateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ApplicationDuplicateResponse> GetByIdAsync(long applicationDuplicateId)
        {
            var entity = await _repository.GetByIdAsync(applicationDuplicateId)
                ?? throw new KeyNotFoundException($"ApplicationDuplicate with ID {applicationDuplicateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ApplicationDuplicateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ApplicationDuplicateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
