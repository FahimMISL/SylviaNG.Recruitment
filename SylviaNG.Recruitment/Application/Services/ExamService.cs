using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamService(IExamRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExamId;
        }

        public async Task UpdateAsync(long examId, ExamUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(examId)
                ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long examId)
        {
            var entity = await _repository.GetByIdAsync(examId)
                ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExamResponse> GetByIdAsync(long examId)
        {
            var entity = await _repository.GetByIdAsync(examId)
                ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExamResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
