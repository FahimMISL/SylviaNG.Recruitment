using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluationScores.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewEvaluationScoreService : IInterviewEvaluationScoreService
    {
        private readonly IInterviewEvaluationScoreRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewEvaluationScoreService(IInterviewEvaluationScoreRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewEvaluationScoreCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewEvaluationScoreId;
        }

        public async Task UpdateAsync(long interviewEvaluationScoreId, InterviewEvaluationScoreUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationScoreId)
                ?? throw new KeyNotFoundException($"InterviewEvaluationScore with ID {interviewEvaluationScoreId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewEvaluationScoreId)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationScoreId)
                ?? throw new KeyNotFoundException($"InterviewEvaluationScore with ID {interviewEvaluationScoreId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewEvaluationScoreResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewEvaluationScoreResponse> GetByIdAsync(long interviewEvaluationScoreId)
        {
            var entity = await _repository.GetByIdAsync(interviewEvaluationScoreId)
                ?? throw new KeyNotFoundException($"InterviewEvaluationScore with ID {interviewEvaluationScoreId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewEvaluationScoreResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewEvaluationScoreResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
