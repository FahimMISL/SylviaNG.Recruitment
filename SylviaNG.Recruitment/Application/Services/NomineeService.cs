using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.Nominees.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class NomineeService : INomineeService
    {
        private readonly INomineeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NomineeService(INomineeRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(NomineeCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.NomineeId;
        }

        public async Task UpdateAsync(long nomineeId, NomineeUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(nomineeId)
                ?? throw new KeyNotFoundException($"Nominee with ID {nomineeId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long nomineeId)
        {
            var entity = await _repository.GetByIdAsync(nomineeId)
                ?? throw new KeyNotFoundException($"Nominee with ID {nomineeId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<NomineeResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<NomineeResponse> GetByIdAsync(long nomineeId)
        {
            var entity = await _repository.GetByIdAsync(nomineeId)
                ?? throw new KeyNotFoundException($"Nominee with ID {nomineeId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<NomineeResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<NomineeResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
