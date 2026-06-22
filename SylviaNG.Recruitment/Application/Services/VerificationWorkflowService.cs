using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.VerificationWorkflows.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class VerificationWorkflowService : IVerificationWorkflowService
    {
        private readonly IVerificationWorkflowRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public VerificationWorkflowService(IVerificationWorkflowRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(VerificationWorkflowCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.VerificationWorkflowId;
        }

        public async Task UpdateAsync(long verificationWorkflowId, VerificationWorkflowUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(verificationWorkflowId)
                ?? throw new KeyNotFoundException($"VerificationWorkflow with ID {verificationWorkflowId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long verificationWorkflowId)
        {
            var entity = await _repository.GetByIdAsync(verificationWorkflowId)
                ?? throw new KeyNotFoundException($"VerificationWorkflow with ID {verificationWorkflowId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<VerificationWorkflowResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<VerificationWorkflowResponse> GetByIdAsync(long verificationWorkflowId)
        {
            var entity = await _repository.GetByIdAsync(verificationWorkflowId)
                ?? throw new KeyNotFoundException($"VerificationWorkflow with ID {verificationWorkflowId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<VerificationWorkflowResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<VerificationWorkflowResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
