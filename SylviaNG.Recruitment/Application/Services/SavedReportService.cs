using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.SavedReports.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class SavedReportService : ISavedReportService
    {
        private readonly ISavedReportRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SavedReportService(ISavedReportRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SavedReportCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.SavedReportId;
        }

        public async Task UpdateAsync(long savedReportId, SavedReportUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(savedReportId)
                ?? throw new KeyNotFoundException($"SavedReport with ID {savedReportId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long savedReportId)
        {
            var entity = await _repository.GetByIdAsync(savedReportId)
                ?? throw new KeyNotFoundException($"SavedReport with ID {savedReportId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<SavedReportResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<SavedReportResponse> GetByIdAsync(long savedReportId)
        {
            var entity = await _repository.GetByIdAsync(savedReportId)
                ?? throw new KeyNotFoundException($"SavedReport with ID {savedReportId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<SavedReportResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<SavedReportResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
