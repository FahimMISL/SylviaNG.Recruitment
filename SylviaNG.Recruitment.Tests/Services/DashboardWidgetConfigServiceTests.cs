using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Dashboard.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class DashboardWidgetConfigServiceTests
{
    private readonly Mock<IDashboardWidgetConfigRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DefaultHttpContext _httpContext;
    private readonly DashboardWidgetConfigService _service;

    public DashboardWidgetConfigServiceTests()
    {
        _repositoryMock = new Mock<IDashboardWidgetConfigRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _httpContext = new DefaultHttpContext();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(a => a.HttpContext).Returns(_httpContext);

        _service = new DashboardWidgetConfigService(_repositoryMock.Object, httpContextAccessorMock.Object, _unitOfWorkMock.Object);
    }

    private void SetRole(string role)
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, role) }, "Test");
        _httpContext.User = new ClaimsPrincipal(identity);
    }

    private static List<DashboardWidgetConfig> SampleConfigs() => new()
    {
        new() { WidgetKey = "OpenVacancies", IsVisibleForAdmin = true, IsVisibleForHR = true },
        new() { WidgetKey = "PendingApprovals", IsVisibleForAdmin = true, IsVisibleForHR = false }
    };

    [Fact]
    public async Task GetVisibleWidgetKeysForCurrentRoleAsync_HrRole_ShouldExcludeHrHiddenWidgets()
    {
        // Arrange
        SetRole("HR");
        _repositoryMock.Setup(r => r.GetAllOrderedAsync()).ReturnsAsync(SampleConfigs());

        // Act
        var result = await _service.GetVisibleWidgetKeysForCurrentRoleAsync();

        // Assert
        result.Should().ContainSingle().Which.Should().Be("OpenVacancies");
    }

    [Fact]
    public async Task GetVisibleWidgetKeysForCurrentRoleAsync_AdminRole_ShouldIncludeAllVisibleForAdminWidgets()
    {
        // Arrange
        SetRole("Admin");
        _repositoryMock.Setup(r => r.GetAllOrderedAsync()).ReturnsAsync(SampleConfigs());

        // Act
        var result = await _service.GetVisibleWidgetKeysForCurrentRoleAsync();

        // Assert
        result.Should().BeEquivalentTo(new[] { "OpenVacancies", "PendingApprovals" });
    }

    [Fact]
    public async Task UpdateVisibilityAsync_UnknownWidgetKey_ShouldThrowNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByKeyAsync("Unknown")).ReturnsAsync((DashboardWidgetConfig?)null);

        // Act
        var act = () => _service.UpdateVisibilityAsync("Unknown", new DashboardWidgetConfigUpdateRequest());

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateVisibilityAsync_KnownWidgetKey_ShouldUpdateBothFlagsAndSave()
    {
        // Arrange
        var entity = new DashboardWidgetConfig { WidgetKey = "OpenVacancies", IsVisibleForAdmin = true, IsVisibleForHR = true };
        _repositoryMock.Setup(r => r.GetByKeyAsync("OpenVacancies")).ReturnsAsync(entity);

        // Act
        await _service.UpdateVisibilityAsync("OpenVacancies", new DashboardWidgetConfigUpdateRequest { IsVisibleForAdmin = true, IsVisibleForHR = false });

        // Assert
        entity.IsVisibleForHR.Should().BeFalse();
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
