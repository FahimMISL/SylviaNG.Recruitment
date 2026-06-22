using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamSeatPlans.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamSeatPlanService : IExamSeatPlanService
    {
        private readonly IExamSeatPlanRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamSeatPlanService(IExamSeatPlanRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamSeatPlanCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExamSeatPlanId;
        }

        public async Task UpdateAsync(long examSeatPlanId, ExamSeatPlanUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(examSeatPlanId)
                ?? throw new KeyNotFoundException($"ExamSeatPlan with ID {examSeatPlanId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long examSeatPlanId)
        {
            var entity = await _repository.GetByIdAsync(examSeatPlanId)
                ?? throw new KeyNotFoundException($"ExamSeatPlan with ID {examSeatPlanId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamSeatPlanResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExamSeatPlanResponse> GetByIdAsync(long examSeatPlanId)
        {
            var entity = await _repository.GetByIdAsync(examSeatPlanId)
                ?? throw new KeyNotFoundException($"ExamSeatPlan with ID {examSeatPlanId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamSeatPlanResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExamSeatPlanResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
