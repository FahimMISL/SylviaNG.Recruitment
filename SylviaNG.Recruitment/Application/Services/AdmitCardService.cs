using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.AdmitCards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class AdmitCardService : IAdmitCardService
    {
        private readonly IAdmitCardRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public AdmitCardService(IAdmitCardRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(AdmitCardCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.AdmitCardId;
        }

        public async Task UpdateAsync(long admitCardId, AdmitCardUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(admitCardId)
                ?? throw new KeyNotFoundException($"AdmitCard with ID {admitCardId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long admitCardId)
        {
            var entity = await _repository.GetByIdAsync(admitCardId)
                ?? throw new KeyNotFoundException($"AdmitCard with ID {admitCardId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<AdmitCardResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<AdmitCardResponse> GetByIdAsync(long admitCardId)
        {
            var entity = await _repository.GetByIdAsync(admitCardId)
                ?? throw new KeyNotFoundException($"AdmitCard with ID {admitCardId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<AdmitCardResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<AdmitCardResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
