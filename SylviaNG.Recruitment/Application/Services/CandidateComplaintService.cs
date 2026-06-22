using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CandidateComplaints.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CandidateComplaintService : ICandidateComplaintService
    {
        private readonly ICandidateComplaintRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CandidateComplaintService(ICandidateComplaintRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CandidateComplaintCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CandidateComplaintId;
        }

        public async Task UpdateAsync(long candidateComplaintId, CandidateComplaintUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(candidateComplaintId)
                ?? throw new KeyNotFoundException($"CandidateComplaint with ID {candidateComplaintId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long candidateComplaintId)
        {
            var entity = await _repository.GetByIdAsync(candidateComplaintId)
                ?? throw new KeyNotFoundException($"CandidateComplaint with ID {candidateComplaintId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CandidateComplaintResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CandidateComplaintResponse> GetByIdAsync(long candidateComplaintId)
        {
            var entity = await _repository.GetByIdAsync(candidateComplaintId)
                ?? throw new KeyNotFoundException($"CandidateComplaint with ID {candidateComplaintId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CandidateComplaintResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CandidateComplaintResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
