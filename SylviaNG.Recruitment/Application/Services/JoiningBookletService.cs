using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.JoiningBooklets.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class JoiningBookletService : IJoiningBookletService
    {
        private readonly IJoiningBookletRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public JoiningBookletService(IJoiningBookletRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(JoiningBookletCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.JoiningBookletId;
        }

        public async Task UpdateAsync(long joiningBookletId, JoiningBookletUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(joiningBookletId)
                ?? throw new KeyNotFoundException($"JoiningBooklet with ID {joiningBookletId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long joiningBookletId)
        {
            var entity = await _repository.GetByIdAsync(joiningBookletId)
                ?? throw new KeyNotFoundException($"JoiningBooklet with ID {joiningBookletId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<JoiningBookletResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<JoiningBookletResponse> GetByIdAsync(long joiningBookletId)
        {
            var entity = await _repository.GetByIdAsync(joiningBookletId)
                ?? throw new KeyNotFoundException($"JoiningBooklet with ID {joiningBookletId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<JoiningBookletResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<JoiningBookletResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
