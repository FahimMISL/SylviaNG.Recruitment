using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JobPostingChannels.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JobPostingChannelService : IJobPostingChannelService
    {
        private readonly IJobPostingChannelRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public JobPostingChannelService(IJobPostingChannelRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(JobPostingChannelCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.JobPostingChannelId;
        }

        public async Task UpdateAsync(long jobPostingChannelId, JobPostingChannelUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(jobPostingChannelId)
                ?? throw new KeyNotFoundException($"JobPostingChannel with ID {jobPostingChannelId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long jobPostingChannelId)
        {
            var entity = await _repository.GetByIdAsync(jobPostingChannelId)
                ?? throw new KeyNotFoundException($"JobPostingChannel with ID {jobPostingChannelId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<JobPostingChannelResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<JobPostingChannelResponse> GetByIdAsync(long jobPostingChannelId)
        {
            var entity = await _repository.GetByIdAsync(jobPostingChannelId)
                ?? throw new KeyNotFoundException($"JobPostingChannel with ID {jobPostingChannelId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<JobPostingChannelResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<JobPostingChannelResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
