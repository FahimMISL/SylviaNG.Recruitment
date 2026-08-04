using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.UserAccounts.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKeycloakClient _keycloakClient;

        public UserAccountService(
            IUserAccountRepository userAccountRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork,
            IKeycloakClient keycloakClient)
        {
            _userAccountRepository = userAccountRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _keycloakClient = keycloakClient;
        }

        public async Task<long> CreateAsync(UserAccountCreateRequest request)
        {
            var exists = await _userAccountRepository.ExistsByEmailAsync(request.Email);
            if (exists)
                throw new DuplicateException("UserAccount", "Email", request.Email);

            var roles = await ResolveRolesAsync(request.RoleIds);
            var (firstName, lastName) = SplitFullName(request.FullName);

            // Keycloak invite flow: create the realm user under the first selected role, then
            // fan the rest of the selected roles onto the same (now-existing) user - Keycloak's
            // CreateUserAsync signature only takes one role at creation time.
            await _keycloakClient.CreateUserAsync(
                username: request.Email,
                email: request.Email,
                firstName: firstName,
                lastName: lastName,
                password: request.Password,
                realmRole: roles[0].Name,
                requireEmailVerification: true);

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
                RoleAssignments = roles.Select(r => new UserRoleAssignment { RoleId = r.RoleId }).ToList()
            };

            await _userAccountRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.UserAccountId;
        }

        public async Task UpdateAsync(long userAccountId, UserAccountUpdateRequest request)
        {
            var entity = await _userAccountRepository.GetByIdWithRolesAsync(userAccountId)
                ?? throw new NotFoundException("UserAccount", userAccountId);

            var roles = await ResolveRolesAsync(request.RoleIds);

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
            var entity = await _userAccountRepository.GetByIdAsync(userAccountId)
                ?? throw new NotFoundException("UserAccount", userAccountId);

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

        private static (string FirstName, string LastName) SplitFullName(string fullName)
        {
            var trimmed = fullName.Trim();
            var spaceIdx = trimmed.IndexOf(' ');
            return spaceIdx < 0
                ? (trimmed, string.Empty)
                : (trimmed[..spaceIdx], trimmed[(spaceIdx + 1)..].Trim());
        }
    }
}
