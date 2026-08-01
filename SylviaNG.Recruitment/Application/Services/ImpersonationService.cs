using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SylviaNG.Recruitment.Application.Common.Authorization;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Impersonation.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    public class ImpersonationService : IImpersonationService
    {
        private static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(30);
        private static readonly string[] ImpersonableSystemRoles = { "Admin", "HR" };

        private readonly IImpersonationSessionRepository _impersonationSessionRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ImpersonationService(
            IImpersonationSessionRepository impersonationSessionRepository,
            IUserAccountRepository userAccountRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _impersonationSessionRepository = impersonationSessionRepository;
            _userAccountRepository = userAccountRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<ImpersonationStartResponse> StartAsync(ImpersonationStartRequest request)
        {
            var actorUserAccountId = await TryGetCurrentUserAccountIdAsync()
                ?? throw new ForbiddenException("Only an invited SuperAdmin user account can start impersonation.");

            var target = await _userAccountRepository.GetByIdWithRolesAsync(request.TargetUserAccountId)
                ?? throw new NotFoundException("UserAccount", request.TargetUserAccountId);

            var targetSystemRoles = target.RoleAssignments
                .Select(a => a.Role.Name)
                .Where(name => ImpersonableSystemRoles.Contains(name))
                .Distinct()
                .ToList();

            if (targetSystemRoles.Count == 0)
                throw new ForbiddenException("Only Admin or HR user accounts can be impersonated.");

            var startedAt = DateTime.UtcNow;
            var expiresAt = startedAt.Add(SessionDuration);

            var session = new ImpersonationSession
            {
                ActorUserAccountId = actorUserAccountId,
                TargetUserAccountId = target.UserAccountId,
                StartedAt = startedAt,
                ExpiresAt = expiresAt
            };

            await _impersonationSessionRepository.AddAsync(session);
            await _unitOfWork.SaveChangesAsync();

            var token = IssueImpersonationToken(target, targetSystemRoles, session.ImpersonationSessionId, actorUserAccountId, expiresAt);

            return new ImpersonationStartResponse
            {
                ImpersonationSessionId = session.ImpersonationSessionId,
                Token = token,
                ExpiresAtUtc = expiresAt,
                TargetFullName = target.FullName,
                TargetEmail = target.Email,
                TargetRole = ImpersonableSystemRoles.First(targetSystemRoles.Contains)
            };
        }

        public async Task EndCurrentAsync()
        {
            var sessionId = GetCurrentImpersonationSessionId()
                ?? throw new ForbiddenException("This request is not using an impersonation token.");

            var session = await _impersonationSessionRepository.GetByIdAsync(sessionId)
                ?? throw new NotFoundException("ImpersonationSession", sessionId);

            if (session.EndedAt is null)
            {
                session.EndedAt = DateTime.UtcNow;
                _impersonationSessionRepository.Update(session);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private string IssueImpersonationToken(UserAccount target, List<string> targetSystemRoles, long sessionId, long actorUserAccountId, DateTime expiresAtUtc)
        {
            var jwtSection = _configuration.GetSection("Jwt:Local");
            var signingKey = jwtSection.GetValue<string>("SigningKey")
                ?? throw new InvalidOperationException("Jwt:Local:SigningKey is not configured.");
            var issuer = jwtSection.GetValue<string>("Issuer") ?? throw new ArgumentNullException("Jwt:Local:Issuer");
            var audience = jwtSection.GetValue<string>("Audience") ?? throw new ArgumentNullException("Jwt:Local:Audience");

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, target.FullName),
                new(ClaimTypes.NameIdentifier, target.KeycloakUserId),
                new(JwtRegisteredClaimNames.Sub, target.KeycloakUserId),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("email", target.Email),
                new(ImpersonationClaimTypes.SessionId, sessionId.ToString()),
                new(ImpersonationClaimTypes.ActorUserAccountId, actorUserAccountId.ToString())
            };
            claims.AddRange(targetSystemRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private long? GetCurrentImpersonationSessionId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(ImpersonationClaimTypes.SessionId)?.Value;
            return long.TryParse(value, out var sessionId) ? sessionId : null;
        }

        private async Task<long?> TryGetCurrentUserAccountIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var keycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(keycloakUserId))
                return null;

            return await _userAccountRepository.GetIdByKeycloakUserIdAsync(keycloakUserId);
        }
    }
}
