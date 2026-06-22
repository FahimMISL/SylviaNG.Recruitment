using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.HiringPipelineStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class HiringPipelineStageService : IHiringPipelineStageService
    {
        private readonly IHiringPipelineStageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public HiringPipelineStageService(IHiringPipelineStageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(HiringPipelineStageCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.HiringPipelineStageId;
        }

        public async Task UpdateAsync(long hiringPipelineStageId, HiringPipelineStageUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(hiringPipelineStageId)
                ?? throw new KeyNotFoundException($"HiringPipelineStage with ID {hiringPipelineStageId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long hiringPipelineStageId)
        {
            var entity = await _repository.GetByIdAsync(hiringPipelineStageId)
                ?? throw new KeyNotFoundException($"HiringPipelineStage with ID {hiringPipelineStageId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<HiringPipelineStageResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<HiringPipelineStageResponse> GetByIdAsync(long hiringPipelineStageId)
        {
            var entity = await _repository.GetByIdAsync(hiringPipelineStageId)
                ?? throw new KeyNotFoundException($"HiringPipelineStage with ID {hiringPipelineStageId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<HiringPipelineStageResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<HiringPipelineStageResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<List<HiringPipelineStageResponse>> GetByJobPostingIdAsync(long jobPostingId)
        {
            var entities = _repository.GetQueryable()
                .Where(e => e.JobPostingId == jobPostingId)
                .OrderBy(e => e.StageOrder)
                .ToList();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
