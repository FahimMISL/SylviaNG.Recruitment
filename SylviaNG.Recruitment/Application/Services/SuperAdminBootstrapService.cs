using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Utils;

namespace SylviaNG.Recruitment.Application.Services
{
    /// <summary>
    /// EP-15 follow-up: fills the "who invites the first SuperAdmin?" gap. UserAccountService's
    /// invite flow needs an existing SuperAdmin to call it, so a brand-new environment (fresh DB +
    /// fresh Keycloak realm) had no way to produce one without someone hand-creating a Keycloak
    /// user and a matching UserAccount row via direct DB/API access.
    ///
    /// Runs once on every startup (see Program.cs), but is a no-op the moment any SuperAdmin
    /// UserAccount exists - so it's safe to leave wired in permanently rather than being a
    /// throwaway migration step. Configured via InitialSuperAdmin:Email/Password/FullName
    /// (appsettings) - if any of those are blank, bootstrap is silently skipped (logged as a
    /// warning) instead of failing startup, since most environments will already have a
    /// SuperAdmin and don't need this section populated at all.
    /// </summary>
    public class SuperAdminBootstrapService : ISuperAdminBootstrapService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKeycloakClient _keycloakClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SuperAdminBootstrapService> _logger;

        public SuperAdminBootstrapService(
            IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IKeycloakClient keycloakClient,
            IConfiguration configuration,
            ILogger<SuperAdminBootstrapService> logger)
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _keycloakClient = keycloakClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            if (await _userAccountRepository.ExistsAnyWithRoleAsync(nameof(UserRoleEnum.SuperAdmin)))
                return;

            var email = _configuration["InitialSuperAdmin:Email"];
            var password = _configuration["InitialSuperAdmin:Password"];
            var fullName = _configuration["InitialSuperAdmin:FullName"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                _logger.LogWarning(
                    "No SuperAdmin account exists and InitialSuperAdmin:Email/Password/FullName is not fully configured - " +
                    "skipping bootstrap. The app has no other way to create the first SuperAdmin.");
                return;
            }

            var role = await _roleRepository.GetByNameAsync(nameof(UserRoleEnum.SuperAdmin))
                ?? throw new InvalidOperationException(
                    "The SuperAdmin system role is missing from the Roles table - run pending EF Core migrations first.");

            var (firstName, lastName) = PersonNameUtility.SplitFullName(fullName);

            // Admin-provisioned, not self-registered - the operator running this already knows
            // the password, so there's no unverified inbox to gate login behind.
            await _keycloakClient.CreateUserAsync(
                username: email,
                email: email,
                firstName: firstName,
                lastName: lastName,
                password: password,
                realmRole: role.Name,
                requireEmailVerification: false);

            var keycloakUserId = await _keycloakClient.GetUserIdByUsernameAsync(email);

            var entity = new UserAccount
            {
                KeycloakUserId = keycloakUserId,
                Email = email,
                FullName = fullName,
                IsActive = true,
                RoleAssignments = new List<UserRoleAssignment> { new() { RoleId = role.RoleId } },
            };

            await _userAccountRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Bootstrapped the initial SuperAdmin account ({Email}).", email);
        }
    }
}
