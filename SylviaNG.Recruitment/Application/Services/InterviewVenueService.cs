using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.InterviewVenues.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class InterviewVenueService : IInterviewVenueService
    {
        private readonly IInterviewVenueRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InterviewVenueService(IInterviewVenueRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(InterviewVenueCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.InterviewVenueId;
        }

        public async Task UpdateAsync(long interviewVenueId, InterviewVenueUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(interviewVenueId)
                ?? throw new KeyNotFoundException($"InterviewVenue with ID {interviewVenueId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long interviewVenueId)
        {
            var entity = await _repository.GetByIdAsync(interviewVenueId)
                ?? throw new KeyNotFoundException($"InterviewVenue with ID {interviewVenueId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<InterviewVenueResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<InterviewVenueResponse> GetByIdAsync(long interviewVenueId)
        {
            var entity = await _repository.GetByIdAsync(interviewVenueId)
                ?? throw new KeyNotFoundException($"InterviewVenue with ID {interviewVenueId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<InterviewVenueResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<InterviewVenueResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
