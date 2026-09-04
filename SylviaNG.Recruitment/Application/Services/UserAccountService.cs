using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SylviaNG.Recruitment.Application.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserInviteOtpRepository _userInviteOtpRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKeycloakClient _keycloakClient;
        private readonly INotificationDispatchService _notificationDispatchService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly OtpSettings _otpSettings;
        private readonly PortalSettings _portalSettings;
        private readonly ILogger<UserAccountService> _logger;

        public UserAccountService(
            IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository,
            ICompanyRepository companyRepository,
            IUserInviteOtpRepository userInviteOtpRepository,
            IUnitOfWork unitOfWork,
            IKeycloakClient keycloakClient,
            INotificationDispatchService notificationDispatchService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<OtpSettings> otpSettings,
            IOptions<PortalSettings> portalSettings,
            ILogger<UserAccountService> logger)
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
            _companyRepository = companyRepository;
            _userInviteOtpRepository = userInviteOtpRepository;
            _unitOfWork = unitOfWork;
            _keycloakClient = keycloakClient;
            _notificationDispatchService = notificationDispatchService;
            _httpContextAccessor = httpContextAccessor;
            _otpSettings = otpSettings.Value;
            _portalSettings = portalSettings.Value;
            _logger = logger;
        }

        public async Task<long> CreateAsync(UserAccountCreateRequest request)
        {
            var exists = await _userAccountRepository.ExistsByEmailAsync(request.Email);
            if (exists)
                throw new DuplicateException("UserAccount", "Email", request.Email);

            var roles = await ResolveRolesAsync(request.RoleIds);
            EnsureCallerCanAssign(roles);
            var companyId = await ResolveAndAuthorizeCompanyAsync(roles, request.CompanyId);
            var (firstName, lastName) = PersonNameUtility.SplitFullName(request.FullName);

            // Creates the Keycloak account with no password set yet - the recipient chooses their
            // own via the accept-invite email this app sends itself below (see UserInviteOtp),
            // not Keycloak's own execute-actions-email (which needs Keycloak's realm SMTP).
            await _keycloakClient.InviteUserAsync(
                username: request.Email,
                email: request.Email,
                firstName: firstName,
                lastName: lastName,
                realmRole: roles[0].Name);

            var keycloakUserId = await _keycloakClient.GetUserIdByUsernameAsync(request.Email);

            if (roles.Count > 1)
            {
                await _keycloakClient.AssignRealmRolesAsync(keycloakUserId, roles.Skip(1).Select(r => r.Name));
            }

            var entity = new UserAccount
            {
                KeycloakUserId = keycloakUserId,
                Email = request.Email,
                FullName = request.FullName,
                IsActive = true,
                CompanyId = companyId,
                RoleAssignments = roles.Select(r => new UserRoleAssignment { RoleId = r.RoleId }).ToList()
            };

            try
            {
                await _userAccountRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // The Keycloak account (and its already-sent invite email) can't be left behind
                // with no local UserAccount row - it would be an orphaned account nobody can
                // manage through this app (RequirePermissionAttribute fails it closed with no
                // local row, so it's locked out either way, but cleanup here beats a silent
                // half-created account). Best-effort/non-throwing - see IKeycloakClient.DeleteUserAsync.
                await _keycloakClient.DeleteUserAsync(keycloakUserId, request.Email);
                throw;
            }

            await SendInviteEmailAsync(keycloakUserId, request.Email);

            return entity.UserAccountId;
        }

        // Delivers the accept-invite OTP + link via this app's own Brevo-backed dispatch,
        // since Keycloak's own execute-actions-email requires Keycloak's realm SMTP (blocked on
        // free hosts the same way this app's own SMTP was - see UserInviteOtp remarks). Same
        // shape as AuthService.BeginOtpChallengeAsync/ForgotPasswordAsync: never lets a dispatch
        // failure fail the whole invite - the UserAccount row and Keycloak user already exist by
        // this point, so an email hiccup here is logged, not thrown.
        private async Task SendInviteEmailAsync(string keycloakUserId, string email)
        {
            var challengeId = Guid.NewGuid();
            var code = GenerateOtpCode();
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_otpSettings.ExpiryMinutes);

            var otp = new UserInviteOtp
            {
                ChallengeId = challengeId,
                KeycloakUserId = keycloakUserId,
                Email = email,
                OtpCodeHash = HashOtpCode(code),
                ExpiresAtUtc = expiresAtUtc
            };

            await _userInviteOtpRepository.AddAsync(otp);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await _notificationDispatchService.DispatchAsync(
                    RecruitmentEventEnum.UserInvited,
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["OtpCode"] = code,
                        ["ExpiryMinutes"] = _otpSettings.ExpiryMinutes.ToString(),
                        // Auth feature module is mounted at '/login' in app.routes.ts, not '/auth' -
                        // same as forgot-password's real URL being '/login/forgot-password'.
                        ["AcceptInviteLink"] = $"{_portalSettings.FrontendBaseUrl}/login/accept-invite?challengeId={challengeId}"
                    },
                    new NotificationDispatchTargets(email, null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error dispatching invite email for challenge {ChallengeId}.", challengeId);
            }
        }

        private static string GenerateOtpCode()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }

        private string HashOtpCode(string code)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_otpSettings.Pepper ?? string.Empty));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(code));
            return Convert.ToHexString(hash);
        }

        // Multi-tenant: SuperAdmin has no company (returns null, ignoring any CompanyId passed
        // in) - every other role (Admin/HR/custom) must belong to exactly one, real, active
        // Company. A non-SuperAdmin caller (a Company Admin inviting HR) is locked to their own
        // company regardless of what CompanyId the request claims, so one company's Admin can
        // never plant a user into another company's tenant.
        private async Task<long?> ResolveAndAuthorizeCompanyAsync(List<Role> roles, long? requestedCompanyId)
        {
            if (roles.All(r => r.Name == nameof(UserRoleEnum.SuperAdmin)))
                return null;

            var caller = _httpContextAccessor.HttpContext?.User;
            var callerIsSuperAdmin = caller?.IsInRole(nameof(UserRoleEnum.SuperAdmin)) ?? false;

            long? companyId;
            if (callerIsSuperAdmin)
            {
                if (!requestedCompanyId.HasValue)
                    throw new ForbiddenException("A company must be selected for this user.");

                companyId = requestedCompanyId.Value;
            }
            else
            {
                var callerKeycloakUserId = caller?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? caller?.FindFirst("sub")?.Value;
                var callerAccount = !string.IsNullOrEmpty(callerKeycloakUserId)
                    ? await _userAccountRepository.GetByKeycloakUserIdWithRolesAsync(callerKeycloakUserId)
                    : null;

                companyId = callerAccount?.CompanyId
                    ?? throw new ForbiddenException("Your account is not associated with a company.");
            }

            var company = await _companyRepository.GetByIdAsync(companyId.Value)
                ?? throw new NotFoundException("Company", companyId.Value);

            if (company.Status != CompanyStatusEnum.Active)
                throw new ForbiddenException("This company is deactivated and cannot receive new users.");

            return companyId;
        }

        public async Task UpdateAsync(long userAccountId, UserAccountUpdateRequest request)
        {
            var entity = await _userAccountRepository.GetByIdWithRolesAsync(userAccountId)
                ?? throw new NotFoundException("UserAccount", userAccountId);

            EnsureCallerCanManage(entity);
            var roles = await ResolveRolesAsync(request.RoleIds);
            EnsureCallerCanAssign(roles);

            entity.FullName = request.FullName;
            entity.RoleAssignments.Clear();
            foreach (var role in roles)
            {
                entity.RoleAssignments.Add(new UserRoleAssignment { RoleId = role.RoleId });
            }

            _userAccountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            // Additive only (see IKeycloakClient.AssignRealmRolesAsync remarks) - a role removed
            // here still exists on the user's Keycloak token until re-invited/manually cleaned up
            // in Keycloak. Acceptable for now: the local RoleAssignments list is what this app's
            // own permission checks read, not the Keycloak token's role claim.
            await _keycloakClient.AssignRealmRolesAsync(entity.KeycloakUserId, roles.Select(r => r.Name));
        }

        public async Task SetActiveAsync(long userAccountId, bool isActive)
        {
            var entity = await _userAccountRepository.GetByIdWithRolesAsync(userAccountId)
                ?? throw new NotFoundException("UserAccount", userAccountId);

            EnsureCallerIsNotDeactivatingSelf(entity, isActive);
            EnsureCallerCanManage(entity);
            entity.IsActive = isActive;
            _userAccountRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserAccountResponse> GetByIdAsync(long userAccountId)
        {
            var entity = await _userAccountRepository.GetByIdWithRolesAsync(userAccountId)
                ?? throw new NotFoundException("UserAccount", userAccountId);

            return entity.ToResponse();
        }

        public async Task<List<UserAccountResponse>> GetAllAsync()
        {
            var entities = await _userAccountRepository.GetAllWithRolesAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        // RequirePermissionAttribute lets an Admin (or a custom role explicitly granted Admin
        // Create/Edit) reach these actions, but RoleIds is caller-supplied - without this check
        // any of them could hand SuperAdmin to a new or existing account (including their own),
        // a straight privilege escalation. Only SuperAdmin may grant the SuperAdmin role.
        private void EnsureCallerCanAssign(List<Role> roles)
        {
            // Candidate is self-service only (register/apply flow, provisions its own
            // CandidateProfile on first login) - it must never be assignable through the
            // staff invite/edit screen, which only manages the local UserAccount table.
            if (roles.Any(r => r.Name == nameof(UserRoleEnum.Candidate)))
                throw new ForbiddenException("The Candidate role cannot be assigned to a staff user account.");

            if (!roles.Any(r => r.Name == nameof(UserRoleEnum.SuperAdmin)))
                return;

            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null || !user.IsInRole(nameof(UserRoleEnum.SuperAdmin)))
                throw new ForbiddenException("Only SuperAdmin can assign the SuperAdmin role.");
        }

        // An Admin may manage ordinary staff accounts, but must never alter a SuperAdmin
        // account. Without this check an Admin could deactivate the highest-privilege account.
        private void EnsureCallerCanManage(UserAccount target)
        {
            if (!target.RoleAssignments.Any(a => a.Role.Name == nameof(UserRoleEnum.SuperAdmin)))
                return;

            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null || !user.IsInRole(nameof(UserRoleEnum.SuperAdmin)))
                throw new ForbiddenException("Only SuperAdmin can manage a SuperAdmin account.");
        }

        // Deactivation is destructive: allowing it for the current principal can permanently
        // lock the only available administrator out of the application.
        private void EnsureCallerIsNotDeactivatingSelf(UserAccount target, bool isActive)
        {
            if (isActive)
                return;

            var user = _httpContextAccessor.HttpContext?.User;
            var currentKeycloakUserId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(currentKeycloakUserId)
                && currentKeycloakUserId == target.KeycloakUserId)
            {
                throw new ForbiddenException("You cannot deactivate your own account.");
            }
        }

        private async Task<List<Role>> ResolveRolesAsync(List<long> roleIds)
        {
            var distinctIds = roleIds.Distinct().ToList();
            var roles = new List<Role>();

            foreach (var roleId in distinctIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId)
                    ?? throw new NotFoundException("Role", roleId);
                roles.Add(role);
            }

            return roles;
        }
    }
}
