using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionOptions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class QuestionOptionService : IQuestionOptionService
    {
        private readonly IQuestionOptionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public QuestionOptionService(IQuestionOptionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(QuestionOptionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.QuestionOptionId;
        }

        public async Task UpdateAsync(long questionOptionId, QuestionOptionUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(questionOptionId)
                ?? throw new KeyNotFoundException($"QuestionOption with ID {questionOptionId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long questionOptionId)
        {
            var entity = await _repository.GetByIdAsync(questionOptionId)
                ?? throw new KeyNotFoundException($"QuestionOption with ID {questionOptionId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<QuestionOptionResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<QuestionOptionResponse> GetByIdAsync(long questionOptionId)
        {
            var entity = await _repository.GetByIdAsync(questionOptionId)
                ?? throw new KeyNotFoundException($"QuestionOption with ID {questionOptionId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<QuestionOptionResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<QuestionOptionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
