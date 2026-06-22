using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentStages.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AssessmentStageService : IAssessmentStageService
    {
        private readonly IAssessmentStageRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentStageService(IAssessmentStageRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(AssessmentStageCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.AssessmentStageId;
        }

        public async Task UpdateAsync(long assessmentStageId, AssessmentStageUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(assessmentStageId)
                ?? throw new KeyNotFoundException($"AssessmentStage with ID {assessmentStageId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long assessmentStageId)
        {
            var entity = await _repository.GetByIdAsync(assessmentStageId)
                ?? throw new KeyNotFoundException($"AssessmentStage with ID {assessmentStageId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AssessmentStageResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<AssessmentStageResponse> GetByIdAsync(long assessmentStageId)
        {
            var entity = await _repository.GetByIdAsync(assessmentStageId)
                ?? throw new KeyNotFoundException($"AssessmentStage with ID {assessmentStageId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<AssessmentStageResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<AssessmentStageResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
