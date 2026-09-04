using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.WaiverRules.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class WaiverRuleService : IWaiverRuleService
    {
        private readonly IWaiverRuleRepository _waiverRuleRepository;
        private readonly ISpecialCategoryRepository _specialCategoryRepository;
        private readonly IReferralSourceRepository _referralSourceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public WaiverRuleService(
            IWaiverRuleRepository waiverRuleRepository,
            ISpecialCategoryRepository specialCategoryRepository,
            IReferralSourceRepository referralSourceRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _waiverRuleRepository = waiverRuleRepository;
            _specialCategoryRepository = specialCategoryRepository;
            _referralSourceRepository = referralSourceRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        private async Task ValidateCriteriaReferencesAsync(long? specialCategoryId, long? referralSourceId)
        {
            if (specialCategoryId.HasValue && await _specialCategoryRepository.GetByIdAsync(specialCategoryId.Value) == null)
                throw new NotFoundException("SpecialCategory", specialCategoryId.Value);

            if (referralSourceId.HasValue && await _referralSourceRepository.GetByIdAsync(referralSourceId.Value) == null)
                throw new NotFoundException("ReferralSource", referralSourceId.Value);
        }

        public async Task<long> CreateAsync(WaiverRuleCreateRequest request)
        {
            var exists = await _waiverRuleRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("WaiverRule", "Name", request.Name);

            await ValidateCriteriaReferencesAsync(request.SpecialCategoryId, request.ReferralSourceId);

            var entity = request.ToEntity();
            entity.CompanyId = await _currentUserService.GetCurrentUserCompanyIdAsync();
            await _waiverRuleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.WaiverRuleId;
        }

        public async Task UpdateAsync(long waiverRuleId, WaiverRuleUpdateRequest request)
        {
            var entity = await _waiverRuleRepository.GetByIdAsync(waiverRuleId)
                ?? throw new NotFoundException("WaiverRule", waiverRuleId);

            var nameTaken = await _waiverRuleRepository.ExistsByNameAsync(request.Name, waiverRuleId);
            if (nameTaken)
                throw new DuplicateException("WaiverRule", "Name", request.Name);

            await ValidateCriteriaReferencesAsync(request.SpecialCategoryId, request.ReferralSourceId);

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.CandidateTypeFilter = request.CandidateTypeFilter;
            entity.SpecialCategoryId = request.SpecialCategoryId;
            entity.ReferralSourceId = request.ReferralSourceId;
            entity.Priority = request.Priority;
            entity.IsActive = request.IsActive;

            _waiverRuleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long waiverRuleId)
        {
            var entity = await _waiverRuleRepository.GetByIdAsync(waiverRuleId)
                ?? throw new NotFoundException("WaiverRule", waiverRuleId);

            // No usage guard needed - JobApplication.WaiverRuleId is DeleteBehavior.SetNull, so
            // deleting a superseded rule never blocks on, or breaks, historical applications.
            _waiverRuleRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<WaiverRuleResponse>> GetAllAsync()
        {
            var entities = await _waiverRuleRepository.GetAllOrderedAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<WaiverRule?> TryMatchAsync(bool candidateIsInternal, long? specialCategoryId, long? referralSourceId)
        {
            var candidateType = candidateIsInternal ? WaiverCandidateTypeEnum.Internal : WaiverCandidateTypeEnum.External;
            var activeRules = await _waiverRuleRepository.GetActiveOrderedByPriorityAsync();

            return activeRules.FirstOrDefault(rule =>
                (rule.CandidateTypeFilter == null || rule.CandidateTypeFilter == candidateType) &&
                (rule.SpecialCategoryId == null || rule.SpecialCategoryId == specialCategoryId) &&
                (rule.ReferralSourceId == null || rule.ReferralSourceId == referralSourceId));
        }
    }
}
