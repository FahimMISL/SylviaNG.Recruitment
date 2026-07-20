using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;
using System.Security.Claims;

namespace SylviaNG.Recruitment.Tests.Services;

public class CurrentCandidateServiceTests
{
    private readonly Mock<ICandidateProfileRepository> _candidateProfileRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentCandidateService _service;

    public CurrentCandidateServiceTests()
    {
        _candidateProfileRepositoryMock = new Mock<ICandidateProfileRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _service = new CurrentCandidateService(
            _httpContextAccessorMock.Object,
            _candidateProfileRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private void SetUpAuthenticatedUser(string subjectId, string name, string email)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, subjectId),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);
    }

    [Fact]
    public async Task GetOrCreateCurrentProfileIdAsync_WhenProfileAlreadyExists_ShouldNotCheckEmployee()
    {
        // Arrange
        SetUpAuthenticatedUser("sub-1", "Alice Rahman", "alice@example.com");
        var existing = new CandidateProfile { CandidateProfileId = 7, KeycloakSubjectId = "sub-1", Email = "alice@example.com" };
        _candidateProfileRepositoryMock.Setup(r => r.GetByKeycloakSubjectIdAsync("sub-1")).ReturnsAsync(existing);

        // Act
        var id = await _service.GetOrCreateCurrentProfileIdAsync();

        // Assert
        id.Should().Be(7);
        _employeeRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _candidateProfileRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CandidateProfile>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateCurrentProfileIdAsync_WhenNewAndEmployeeMatchesByEmail_ShouldPrepopulateFromCoreHr()
    {
        // Arrange
        SetUpAuthenticatedUser("sub-2", "A Rahman", "bilal@example.com");
        _candidateProfileRepositoryMock.Setup(r => r.GetByKeycloakSubjectIdAsync("sub-2")).ReturnsAsync((CandidateProfile?)null);

        var employee = new Employee
        {
            EmployeeId = 42,
            EmployeeName = "Bilal Hossain",
            Phone = "01711111111",
            DepartmentId = 5,
            DesignatioId = 9,
        };
        _employeeRepositoryMock.Setup(r => r.GetByEmailAsync("bilal@example.com")).ReturnsAsync(employee);

        CandidateProfile? added = null;
        _candidateProfileRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CandidateProfile>()))
            .Callback<CandidateProfile>(p => added = p)
            .Returns(Task.CompletedTask);

        // Act
        await _service.GetOrCreateCurrentProfileIdAsync();

        // Assert
        added.Should().NotBeNull();
        added!.EmployeeId.Should().Be(42);
        added.DepartmentId.Should().Be(5);
        added.DesignationId.Should().Be(9);
        added.FullName.Should().Be("Bilal Hossain");
        added.Phone.Should().Be("01711111111");
        added.PrepopulatedFullName.Should().Be("Bilal Hossain");
        added.PrepopulatedPhone.Should().Be("01711111111");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateCurrentProfileIdAsync_WhenNewAndNoEmployeeMatch_ShouldProvisionAsExternal()
    {
        // Arrange
        SetUpAuthenticatedUser("sub-3", "Carol External", "carol@example.com");
        _candidateProfileRepositoryMock.Setup(r => r.GetByKeycloakSubjectIdAsync("sub-3")).ReturnsAsync((CandidateProfile?)null);
        _employeeRepositoryMock.Setup(r => r.GetByEmailAsync("carol@example.com")).ReturnsAsync((Employee?)null);

        CandidateProfile? added = null;
        _candidateProfileRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CandidateProfile>()))
            .Callback<CandidateProfile>(p => added = p)
            .Returns(Task.CompletedTask);

        // Act
        await _service.GetOrCreateCurrentProfileIdAsync();

        // Assert
        added.Should().NotBeNull();
        added!.EmployeeId.Should().BeNull();
        added.FullName.Should().Be("Carol External");
        added.PrepopulatedFullName.Should().BeNull();
    }
}
