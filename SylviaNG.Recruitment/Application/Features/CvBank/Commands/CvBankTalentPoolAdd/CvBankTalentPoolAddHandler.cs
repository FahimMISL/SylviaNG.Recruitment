using MediatR;
using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Features.CvBank.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Features.CvBank.Commands.CvBankTalentPoolAdd
{
    public class CvBankTalentPoolAddHandler : IRequestHandler<CvBankTalentPoolAddCommand, CvBankTalentPoolAddResponse>
    {
        private readonly ICandidateTalentPoolRepository _candidateTalentPoolRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public CvBankTalentPoolAddHandler(
            ICandidateTalentPoolRepository candidateTalentPoolRepository,
            IUserAccountRepository userAccountRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor? httpContextAccessor = null)
        {
            _candidateTalentPoolRepository = candidateTalentPoolRepository;
            _userAccountRepository = userAccountRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CvBankTalentPoolAddResponse> Handle(CvBankTalentPoolAddCommand command, CancellationToken cancellationToken)
        {
            var requestedIds = command.Request.CandidateProfileIds.Distinct().ToList();
            var existingIds = await _candidateTalentPoolRepository.GetExistingCandidateProfileIdsAsync(requestedIds);

            var toAdd = requestedIds.Where(id => !existingIds.Contains(id)).ToList();
            var companyId = await TryGetCurrentUserCompanyIdAsync();

            foreach (var candidateProfileId in toAdd)
            {
                await _candidateTalentPoolRepository.AddAsync(new CandidateTalentPool
                {
                    CandidateProfileId = candidateProfileId,
                    CompanyId = companyId,
                });
            }

            if (toAdd.Count > 0)
                await _unitOfWork.SaveChangesAsync();

            return new CvBankTalentPoolAddResponse
            {
                AddedCount = toAdd.Count,
                AlreadyInPoolCount = existingIds.Count
            };
        }

        private async Task<long?> TryGetCurrentUserCompanyIdAsync()
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            var account = await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(keycloakUserId);
            return account?.CompanyId;
        }
    }
}
