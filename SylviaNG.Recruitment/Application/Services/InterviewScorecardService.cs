using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewScorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewScorecardService : IInterviewScorecardService
    {
        private readonly IInterviewScorecardRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewScorecardService(IInterviewScorecardRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewScorecardCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewScorecardId;
        }

        public async Task UpdateAsync(long interviewScorecardId, InterviewScorecardUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardId)
                ?? throw new KeyNotFoundException($"InterviewScorecard with ID {interviewScorecardId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewScorecardId)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardId)
                ?? throw new KeyNotFoundException($"InterviewScorecard with ID {interviewScorecardId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewScorecardResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewScorecardResponse> GetByIdAsync(long interviewScorecardId)
        {
            var entity = await _repository.GetByIdAsync(interviewScorecardId)
                ?? throw new KeyNotFoundException($"InterviewScorecard with ID {interviewScorecardId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewScorecardResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewScorecardResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
