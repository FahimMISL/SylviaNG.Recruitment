using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewSessions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewSessionService : IInterviewSessionService
    {
        private readonly IInterviewSessionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewSessionService(IInterviewSessionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewSessionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewSessionId;
        }

        public async Task UpdateAsync(long interviewSessionId, InterviewSessionUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewSessionId)
                ?? throw new KeyNotFoundException($"InterviewSession with ID {interviewSessionId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewSessionId)
        {
            var entity = await _repository.GetByIdAsync(interviewSessionId)
                ?? throw new KeyNotFoundException($"InterviewSession with ID {interviewSessionId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewSessionResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewSessionResponse> GetByIdAsync(long interviewSessionId)
        {
            var entity = await _repository.GetByIdAsync(interviewSessionId)
                ?? throw new KeyNotFoundException($"InterviewSession with ID {interviewSessionId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewSessionResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewSessionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
