using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class QuestionGroupService : IQuestionGroupService
    {
        private readonly IQuestionGroupRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public QuestionGroupService(IQuestionGroupRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(QuestionGroupCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.QuestionGroupId;
        }

        public async Task UpdateAsync(long questionGroupId, QuestionGroupUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(questionGroupId)
                ?? throw new KeyNotFoundException($"QuestionGroup with ID {questionGroupId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long questionGroupId)
        {
            var entity = await _repository.GetByIdAsync(questionGroupId)
                ?? throw new KeyNotFoundException($"QuestionGroup with ID {questionGroupId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<QuestionGroupResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<QuestionGroupResponse> GetByIdAsync(long questionGroupId)
        {
            var entity = await _repository.GetByIdAsync(questionGroupId)
                ?? throw new KeyNotFoundException($"QuestionGroup with ID {questionGroupId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<QuestionGroupResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<QuestionGroupResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
