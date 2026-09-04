using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Tests.Services;

public class UserAccountServiceTests
{
    private readonly Mock<IUserAccountRepository> _userAccountRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UserAccountService _service;

    public UserAccountServiceTests()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, "current-user")], "Test"))
        };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(a => a.HttpContext).Returns(httpContext);

        _service = new UserAccountService(
            _userAccountRepositoryMock.Object,
            new Mock<IRoleRepository>().Object,
            new Mock<ICompanyRepository>().Object,
            new Mock<IUserInviteOtpRepository>().Object,
            _unitOfWorkMock.Object,
            new Mock<IKeycloakClient>().Object,
            new Mock<INotificationDispatchService>().Object,
            httpContextAccessor.Object,
            Options.Create(new OtpSettings()),
            Options.Create(new PortalSettings()),
            NullLogger<UserAccountService>.Instance);
    }

    [Fact]
    public async Task SetActiveAsync_WhenDeactivatingSelf_ThrowsAndDoesNotPersist()
    {
        var account = new UserAccount { UserAccountId = 7, KeycloakUserId = "current-user", IsActive = true };
        _userAccountRepositoryMock.Setup(r => r.GetByIdWithRolesAsync(7)).ReturnsAsync(account);

        var act = () => _service.SetActiveAsync(7, false);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("*cannot deactivate your own account*");
        account.IsActive.Should().BeTrue();
        _userAccountRepositoryMock.Verify(r => r.Update(It.IsAny<UserAccount>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task SetActiveAsync_WhenDeactivatingAnotherStaffAccount_Persists()
    {
        var account = new UserAccount { UserAccountId = 8, KeycloakUserId = "another-user", IsActive = true };
        _userAccountRepositoryMock.Setup(r => r.GetByIdWithRolesAsync(8)).ReturnsAsync(account);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.SetActiveAsync(8, false);

        account.IsActive.Should().BeFalse();
        _userAccountRepositoryMock.Verify(r => r.Update(account), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
