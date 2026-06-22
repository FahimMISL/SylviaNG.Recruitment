using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewEvaluationService : IInterviewEvaluationService
    {
        private readonly IInterviewEvaluationRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewEvaluationService(IInterviewEvaluationRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewEvaluationCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewEvaluationId;
        }

        public async Task UpdateAsync(long interviewEvaluationId, InterviewEvaluationUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationId)
                ?? throw new KeyNotFoundException($"InterviewEvaluation with ID {interviewEvaluationId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewEvaluationId)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationId)
                ?? throw new KeyNotFoundException($"InterviewEvaluation with ID {interviewEvaluationId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewEvaluationResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewEvaluationResponse> GetByIdAsync(long interviewEvaluationId)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationId)
                ?? throw new KeyNotFoundException($"InterviewEvaluation with ID {interviewEvaluationId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewEvaluationResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewEvaluationResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
