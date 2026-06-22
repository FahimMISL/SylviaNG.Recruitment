using SylviaNG.Recruitment.SharedKernel.Pagination;
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
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RoleCreateRequest request)
        {
            var exists = await _roleRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Role", "Name", request.Name);

            var entity = request.ToEntity();
            await _roleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.RoleId;
        }

        public async Task UpdateAsync(long roleId, RoleUpdateRequest request)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            if (request.Name != null)
            {
                var exists = await _roleRepository.ExistsByNameAsync(request.Name, roleId);
                if (exists)
                    throw new DuplicateException("Role", "Name", request.Name);
            }

            entity.ApplyUpdate(request);
            _roleRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long roleId)
        {
            var entity = await _roleRepository.GetByIdAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

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
            var entities = await _roleRepository.GetAllWithIncludeAsync(
                r => r.RolePermissions);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task AssignPermissionsAsync(long roleId, RolePermissionAssignRequest request)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(roleId)
                ?? throw new NotFoundException("Role", roleId);

            role.RolePermissions.Clear();

            foreach (var permissionId in request.PermissionIds)
            {
                var permission = await _permissionRepository.GetByIdAsync(permissionId)
                    ?? throw new NotFoundException("Permission", permissionId);

                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    IsActive = true
                });
            }

            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<long> CreatePermissionAsync(PermissionCreateRequest request)
        {
            var exists = await _permissionRepository.ExistsByModuleAndActionAsync(request.Module, request.Action);
            if (exists)
                throw new DuplicateException("Permission", "Module+Action", $"{request.Module}.{request.Action}");

            var entity = request.ToEntity();
            await _permissionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.PermissionId;
        }

        public async Task<List<PermissionResponse>> GetAllPermissionsAsync()
        {
            var entities = await _permissionRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<RoleResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _roleRepository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<RoleResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
