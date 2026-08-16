using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ReferralSources.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ReferralSourceService : IReferralSourceService
    {
        private readonly IReferralSourceRepository _referralSourceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ReferralSourceService(IReferralSourceRepository referralSourceRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _referralSourceRepository = referralSourceRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(ReferralSourceCreateRequest request)
        {
            var exists = await _referralSourceRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("ReferralSource", "Name", request.Name);

            var entity = request.ToEntity();
            entity.CompanyId = await _currentUserService.GetCurrentUserCompanyIdAsync();
            await _referralSourceRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReferralSourceId;
        }

        public async Task UpdateAsync(long referralSourceId, ReferralSourceUpdateRequest request)
        {
            var entity = await _referralSourceRepository.GetByIdAsync(referralSourceId)
                ?? throw new NotFoundException("ReferralSource", referralSourceId);

            var nameTaken = await _referralSourceRepository.ExistsByNameAsync(request.Name, referralSourceId);
            if (nameTaken)
                throw new DuplicateException("ReferralSource", "Name", request.Name);

            entity.Name = request.Name;
            _referralSourceRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long referralSourceId)
        {
            var entity = await _referralSourceRepository.GetByIdAsync(referralSourceId)
                ?? throw new NotFoundException("ReferralSource", referralSourceId);

            var usageCount = await _referralSourceRepository.CountUsageAsync(referralSourceId);
            if (usageCount > 0)
                throw new ResourceInUseException("ReferralSource", referralSourceId, usageCount);

            _referralSourceRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ReferralSourceResponse>> GetAllAsync()
        {
            var entities = await _referralSourceRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
