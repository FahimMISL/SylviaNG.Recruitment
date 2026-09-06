using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKeycloakClient _keycloakClient;

        public RoleService(IRoleRepository roleRepository, IUnitOfWork unitOfWork, IKeycloakClient keycloakClient)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _keycloakClient = keycloakClient;
        }

        public async Task<long> CreateAsync(RoleCreateRequest request)
        {
            var exists = await _roleRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Role", "Name", request.Name);

            var entity = request.ToEntity();
            await _roleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Custom roles need a matching Keycloak realm role to actually be assignable to a
            // user's token - created after the local row so a Keycloak outage doesn't leave an
            // orphaned realm role with no local Role behind it.
            await _keycloakClient.CreateRealmRoleAsync(entity.Name);

            return entity.RoleId;
        }

        public async Task UpdateAsync(long roleId, RoleUpdateRequest request)
        {
            var entity = await _roleRepository.GetByIdWithPermissionsAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            if (entity.IsSystemRole)
                throw new ForbiddenException("System roles cannot be edited.");

            var nameTaken = await _roleRepository.ExistsByNameAsync(request.Name, roleId);
            if (nameTaken)
                throw new DuplicateException("Role", "Name", request.Name);

            entity.Name = request.Name;
            entity.Permissions.Clear();
            foreach (var grant in request.Permissions)
            {
                entity.Permissions.Add(new RolePermission { Module = grant.Module, Action = grant.Action });
            }

            _roleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long roleId)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            if (entity.IsSystemRole)
                throw new ForbiddenException("System roles cannot be deleted.");

            var usageCount = await _roleRepository.CountAssignedUsersAsync(roleId);
            if (usageCount > 0)
                throw new ResourceInUseException("Role", roleId, usageCount);

            _roleRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RoleResponse> GetByIdAsync(long roleId)
        {
            var entity = await _roleRepository.GetByIdWithPermissionsAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            return entity.ToResponse();
        }

        public async Task<List<RoleResponse>> GetAllAsync()
        {
            var entities = await _roleRepository.GetAllWithPermissionsAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
