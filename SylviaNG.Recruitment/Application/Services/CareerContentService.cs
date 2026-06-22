using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.CareerContents.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CareerContentService : ICareerContentService
    {
        private readonly ICareerContentRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CareerContentService(ICareerContentRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(CareerContentCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.CareerContentId;
        }

        public async Task UpdateAsync(long careerContentId, CareerContentUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(careerContentId)
                ?? throw new KeyNotFoundException($"CareerContent with ID {careerContentId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long careerContentId)
        {
            var entity = await _repository.GetByIdAsync(careerContentId)
                ?? throw new KeyNotFoundException($"CareerContent with ID {careerContentId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CareerContentResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<CareerContentResponse> GetByIdAsync(long careerContentId)
        {
            var entity = await _repository.GetByIdAsync(careerContentId)
                ?? throw new KeyNotFoundException($"CareerContent with ID {careerContentId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<CareerContentResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<CareerContentResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
