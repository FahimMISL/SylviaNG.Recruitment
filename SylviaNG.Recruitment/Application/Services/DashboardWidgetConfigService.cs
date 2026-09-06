using Microsoft.AspNetCore.Http;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Extensions;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Application.Services
{
    public class DashboardWidgetConfigService : IDashboardWidgetConfigService
    {
        private readonly IDashboardWidgetConfigRepository _dashboardWidgetConfigRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public DashboardWidgetConfigService(
            IDashboardWidgetConfigRepository dashboardWidgetConfigRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _dashboardWidgetConfigRepository = dashboardWidgetConfigRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<string>> GetVisibleWidgetKeysForCurrentRoleAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("No authenticated user in the current request.");
            var role = user.GetHighestRole();

            var all = await _dashboardWidgetConfigRepository.GetAllOrderedAsync();
            return all
                .Where(w => role == UserRoleEnum.Admin ? w.IsVisibleForAdmin : w.IsVisibleForHR)
                .Select(w => w.WidgetKey)
                .ToList();
        }

        public async Task<List<DashboardWidgetConfigResponse>> GetAllAsync()
        {
            var all = await _dashboardWidgetConfigRepository.GetAllOrderedAsync();
            return all.Select(w => new DashboardWidgetConfigResponse
            {
                WidgetKey = w.WidgetKey,
                IsVisibleForAdmin = w.IsVisibleForAdmin,
                IsVisibleForHR = w.IsVisibleForHR
            }).ToList();
        }

        public async Task UpdateVisibilityAsync(string widgetKey, DashboardWidgetConfigUpdateRequest request)
        {
            var entity = await _dashboardWidgetConfigRepository.GetByKeyAsync(widgetKey)
                ?? throw new NotFoundException("DashboardWidgetConfig", widgetKey);

            entity.IsVisibleForAdmin = request.IsVisibleForAdmin;
            entity.IsVisibleForHR = request.IsVisibleForHR;
            _dashboardWidgetConfigRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
