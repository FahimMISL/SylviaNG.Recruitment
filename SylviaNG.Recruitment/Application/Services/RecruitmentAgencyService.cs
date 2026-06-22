using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Features.RecruitmentAgencies.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RecruitmentAgencyService : IRecruitmentAgencyService
    {
        private readonly IRecruitmentAgencyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecruitmentAgencyService(IRecruitmentAgencyRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RecruitmentAgencyCreateRequest request)
        {
            var entity = request.ToEntity();
            await _repository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity.RecruitmentAgencyId;
        }

        public async Task UpdateAsync(long recruitmentAgencyId, RecruitmentAgencyUpdateRequest request)
        {
            var entity = await _repository.GetByIdAsync(recruitmentAgencyId)
                ?? throw new KeyNotFoundException($"RecruitmentAgency with ID {recruitmentAgencyId} not found.");
            entity.ApplyUpdate(request);
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long recruitmentAgencyId)
        {
            var entity = await _repository.GetByIdAsync(recruitmentAgencyId)
                ?? throw new KeyNotFoundException($"RecruitmentAgency with ID {recruitmentAgencyId} not found.");
            _repository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RecruitmentAgencyResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<RecruitmentAgencyResponse> GetByIdAsync(long recruitmentAgencyId)
        {
            var entity = await _repository.GetByIdAsync(recruitmentAgencyId)
                ?? throw new KeyNotFoundException($"RecruitmentAgency with ID {recruitmentAgencyId} not found.");
            return entity.ToResponse();
        }

        public async Task<PagedResult<RecruitmentAgencyResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _repository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RecruitmentAgencyResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
