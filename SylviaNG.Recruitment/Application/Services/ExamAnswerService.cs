using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamAnswers.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamAnswerService : IExamAnswerService
    {
        private readonly IExamAnswerRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamAnswerService(IExamAnswerRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamAnswerCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExamAnswerId;
        }

        public async Task UpdateAsync(long examAnswerId, ExamAnswerUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(examAnswerId)
                ?? throw new KeyNotFoundException($"ExamAnswer with ID {examAnswerId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long examAnswerId)
        {
            var entity = await _repository.GetByIdAsync(examAnswerId)
                ?? throw new KeyNotFoundException($"ExamAnswer with ID {examAnswerId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamAnswerResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExamAnswerResponse> GetByIdAsync(long examAnswerId)
        {
            var entity = await _repository.GetByIdAsync(examAnswerId)
                ?? throw new KeyNotFoundException($"ExamAnswer with ID {examAnswerId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamAnswerResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExamAnswerResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
