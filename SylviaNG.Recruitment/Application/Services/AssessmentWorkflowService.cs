using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AssessmentWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AssessmentWorkflowService : IAssessmentWorkflowService
    {
        private readonly IAssessmentWorkflowRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AssessmentWorkflowService(IAssessmentWorkflowRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(AssessmentWorkflowCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.AssessmentWorkflowId;
        }

        public async Task UpdateAsync(long assessmentWorkflowId, AssessmentWorkflowUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(assessmentWorkflowId)
                ?? throw new KeyNotFoundException($"AssessmentWorkflow with ID {assessmentWorkflowId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long assessmentWorkflowId)
        {
            var entity = await _repository.GetByIdAsync(assessmentWorkflowId)
                ?? throw new KeyNotFoundException($"AssessmentWorkflow with ID {assessmentWorkflowId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AssessmentWorkflowResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<AssessmentWorkflowResponse> GetByIdAsync(long assessmentWorkflowId)
        {
            var entity = await _repository.GetByIdAsync(assessmentWorkflowId)
                ?? throw new KeyNotFoundException($"AssessmentWorkflow with ID {assessmentWorkflowId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<AssessmentWorkflowResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<AssessmentWorkflowResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
