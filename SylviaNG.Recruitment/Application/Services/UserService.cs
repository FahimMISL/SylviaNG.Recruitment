using SylviaNG.Recruitment.SharedKernel.Pagination;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Roles.Models;
using SylviaNG.Recruitment.Application.Features.Users.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Mappings;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(UserCreateRequest request)
        {
            var exists = await _userRepository.ExistsByKeycloakIdAsync(request.KeycloakUserId);
            if (exists)
                throw new DuplicateException("User", "KeycloakUserId", request.KeycloakUserId);

            var entity = request.ToEntity();
            await _userRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.UserId;
        }

        public async Task UpdateAsync(long userId, UserUpdateRequest request)
        {
            var entity = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            entity.ApplyUpdate(request);
            _userRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long userId)
        {
            var entity = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            _userRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<UserResponse> GetByIdAsync(long userId)
        {
            var entity = await _userRepository.GetByIdWithRolesAsync(userId)
                ?? throw new NotFoundException("User", userId);

            return entity.ToResponse();
        }

        public async Task<List<UserResponse>> GetAllAsync()
        {
            var entities = await _userRepository.GetAllWithIncludeAsync(
                u => u.UserRoles);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task AssignRoleAsync(long userId, UserRoleAssignRequest request)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var role = await _roleRepository.GetByIdAsync(request.RoleId)
                ?? throw new NotFoundException("Role", request.RoleId);

            var alreadyAssigned = user.UserRoles.Any(ur => ur.RoleId == request.RoleId);
            if (alreadyAssigned)
                throw new DuplicateException("UserRole", "RoleId", request.RoleId.ToString());

            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = request.RoleId,
                ScopeBusinessUnitId = request.ScopeBusinessUnitId,
                ScopeDepartmentId = request.ScopeDepartmentId,
                IsActive = true
            });

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(long userId, long roleId)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId)
                ?? throw new NotFoundException("UserRole", roleId);

            user.UserRoles.Remove(userRole);
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<UserResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _userRepository.GetQueryable().ToPaginatedResultAsync(request);
            return new PagedResult<UserResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
