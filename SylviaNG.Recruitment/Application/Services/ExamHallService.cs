using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.ExamHalls.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ExamHallService : IExamHallService
    {
        private readonly IExamHallRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ExamHallService(IExamHallRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ExamHallCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.ExamHallId;
        }

        public async Task UpdateAsync(long examHallId, ExamHallUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(examHallId)
                ?? throw new KeyNotFoundException($"ExamHall with ID {examHallId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long examHallId)
        {
            var entity = await _repository.GetByIdAsync(examHallId)
                ?? throw new KeyNotFoundException($"ExamHall with ID {examHallId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ExamHallResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<ExamHallResponse> GetByIdAsync(long examHallId)
        {
            var entity = await _repository.GetByIdAsync(examHallId)
                ?? throw new KeyNotFoundException($"ExamHall with ID {examHallId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<ExamHallResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<ExamHallResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
