using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Application.Services
{
    public class CurrentCandidateService : ICurrentCandidateService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICandidateProfileRepository _candidateProfileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CurrentCandidateService(
            IHttpContextAccessor httpContextAccessor,
            ICandidateProfileRepository candidateProfileRepository,
            IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _candidateProfileRepository = candidateProfileRepository;
            _unitOfWork = unitOfWork;
        }

        public string GetCurrentKeycloakSubjectId()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user in the current request.");

            // Keycloak's "sub" claim is remapped to ClaimTypes.NameIdentifier by the default
            // JWT bearer inbound claim map (see AuthenticationExtensions - MapInboundClaims is
            // never disabled). It is a GUID string, not the numeric long id that
            // JobPostingService.TryGetCurrentUserId expects, so it is kept as a string here.
            var subjectId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(subjectId))
                throw new UnauthorizedAccessException("Authenticated token does not carry a subject (sub) claim.");

            return subjectId;
        }

        public async Task<long> GetOrCreateCurrentProfileIdAsync()
        {
            var profile = await GetOrCreateCurrentProfileAsync();
            return profile.CandidateProfileId;
        }

        public async Task<string> GetCurrentEmailAsync()
        {
            var profile = await GetOrCreateCurrentProfileAsync();
            return profile.Email;
        }

        private async Task<CandidateProfile> GetOrCreateCurrentProfileAsync()
        {
            var subjectId = GetCurrentKeycloakSubjectId();

            var existing = await _candidateProfileRepository.GetByKeycloakSubjectIdAsync(subjectId);
            if (existing != null)
                return existing;

            var user = _httpContextAccessor.HttpContext!.User;
            var fullName = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value ?? string.Empty;
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst("email")?.Value ?? string.Empty;

            var profile = new CandidateProfile
            {
                KeycloakSubjectId = subjectId,
                FullName = fullName,
                Email = email,
                IsActive = true
            };

            await _candidateProfileRepository.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            return profile;
        }
    }
}
