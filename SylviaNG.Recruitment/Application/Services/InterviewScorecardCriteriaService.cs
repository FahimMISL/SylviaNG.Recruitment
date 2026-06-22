using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecardCriterias.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewScorecardCriteriaService : IInterviewScorecardCriteriaService
    {
        private readonly IInterviewScorecardCriteriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewScorecardCriteriaService(IInterviewScorecardCriteriaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewScorecardCriteriaCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewScorecardCriteriaId;
        }

        public async Task UpdateAsync(long interviewScorecardCriteriaId, InterviewScorecardCriteriaUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardCriteriaId)
                ?? throw new KeyNotFoundException($"InterviewScorecardCriteria with ID {interviewScorecardCriteriaId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewScorecardCriteriaId)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardCriteriaId)
                ?? throw new KeyNotFoundException($"InterviewScorecardCriteria with ID {interviewScorecardCriteriaId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewScorecardCriteriaResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewScorecardCriteriaResponse> GetByIdAsync(long interviewScorecardCriteriaId)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardCriteriaId)
                ?? throw new KeyNotFoundException($"InterviewScorecardCriteria with ID {interviewScorecardCriteriaId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewScorecardCriteriaResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewScorecardCriteriaResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
