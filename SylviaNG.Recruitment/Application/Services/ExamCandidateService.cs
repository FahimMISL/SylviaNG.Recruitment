using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamCandidates.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamCandidateService : IExamCandidateService
    {
        private readonly IExamCandidateRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamCandidateService(IExamCandidateRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamCandidateCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExamCandidateId;
        }

        public async Task UpdateAsync(long examCandidateId, ExamCandidateUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(examCandidateId)
                ?? throw new KeyNotFoundException($"ExamCandidate with ID {examCandidateId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long examCandidateId)
        {
            var entity = await _repository.GetByIdAsync(examCandidateId)
                ?? throw new KeyNotFoundException($"ExamCandidate with ID {examCandidateId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamCandidateResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExamCandidateResponse> GetByIdAsync(long examCandidateId)
        {
            var entity = await _repository.GetByIdAsync(examCandidateId)
                ?? throw new KeyNotFoundException($"ExamCandidate with ID {examCandidateId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamCandidateResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExamCandidateResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
