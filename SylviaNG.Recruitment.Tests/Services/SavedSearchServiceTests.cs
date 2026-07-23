using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.SavedSearches.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class SavedSearchServiceTests
{
    private readonly Mock<ISavedSearchRepository> _savedSearchRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SavedSearchService _service;

    public SavedSearchServiceTests()
    {
        _savedSearchRepositoryMock = new Mock<ISavedSearchRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _currentUserServiceMock.Setup(c => c.GetCurrentUserName()).Returns("abir");
        _currentUserServiceMock.Setup(c => c.IsInRole("Admin")).Returns(false);

        _service = new SavedSearchService(_savedSearchRepositoryMock.Object, _currentUserServiceMock.Object, _unitOfWorkMock.Object);
    }

    private static SavedSearchCreateRequest CreateRequest(string name = "My Shortlist", bool isShared = false)
    {
        return new SavedSearchCreateRequest
        {
            Name = name,
            IsShared = isShared,
            FilterJson = "{\"filterLocation\":\"Dhaka\"}"
        };
    }

    [Fact]
    public async Task CreateAsync_WithUniqueName_ShouldSaveWithOwnerAndReturnId()
    {
        // Arrange
        _savedSearchRepositoryMock.Setup(r => r.ExistsByNameForOwnerAsync("abir", "My Shortlist", null)).ReturnsAsync(false);

        SavedSearch? saved = null;
        _savedSearchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<SavedSearch>()))
            .Callback<SavedSearch>(s => { s.SavedSearchId = 10; saved = s; })
            .Returns(Task.CompletedTask);

        // Act
        var id = await _service.CreateAsync(CreateRequest());

        // Assert
        id.Should().Be(10);
        saved.Should().NotBeNull();
        saved!.OwnerUserName.Should().Be("abir");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateNameForOwner_ShouldThrowDuplicateException()
    {
        // Arrange
        _savedSearchRepositoryMock.Setup(r => r.ExistsByNameForOwnerAsync("abir", "My Shortlist", null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(CreateRequest());

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _savedSearchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SavedSearch>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithNoResolvableCurrentUser_ShouldThrowValidationException()
    {
        // Arrange
        _currentUserServiceMock.Setup(c => c.GetCurrentUserName()).Returns((string?)null);

        // Act
        var act = () => _service.CreateAsync(CreateRequest());

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
        _savedSearchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SavedSearch>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((SavedSearch?)null);

        // Act
        var act = () => _service.UpdateAsync(99, new SavedSearchUpdateRequest { Name = "X", FilterJson = "{}" });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ByNonOwnerNonAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var entity = new SavedSearch { SavedSearchId = 1, Name = "Old Name", OwnerUserName = "sadia", FilterJson = "{}" };
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act - current user is "abir", entity owned by "sadia"
        var act = () => _service.UpdateAsync(1, new SavedSearchUpdateRequest { Name = "New Name", FilterJson = "{}" });

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task UpdateAsync_ByAdminOnAnotherUsersSearch_ShouldSucceed()
    {
        // Arrange
        var entity = new SavedSearch { SavedSearchId = 1, Name = "Old Name", OwnerUserName = "sadia", FilterJson = "{}" };
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);
        _savedSearchRepositoryMock.Setup(r => r.ExistsByNameForOwnerAsync("sadia", "New Name", 1)).ReturnsAsync(false);
        _currentUserServiceMock.Setup(c => c.IsInRole("Admin")).Returns(true);

        // Act
        await _service.UpdateAsync(1, new SavedSearchUpdateRequest { Name = "New Name", IsShared = true, FilterJson = "{\"x\":1}" });

        // Assert
        entity.Name.Should().Be("New Name");
        entity.IsShared.Should().BeTrue();
        entity.FilterJson.Should().Be("{\"x\":1}");
        _savedSearchRepositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((SavedSearch?)null);

        // Act
        var act = () => _service.DeleteAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ByNonOwnerNonAdmin_ShouldThrowForbiddenException()
    {
        // Arrange
        var entity = new SavedSearch { SavedSearchId = 1, Name = "X", OwnerUserName = "sadia", FilterJson = "{}" };
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var act = () => _service.DeleteAsync(1);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        _savedSearchRepositoryMock.Verify(r => r.Delete(It.IsAny<SavedSearch>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ByOwner_ShouldDelete()
    {
        // Arrange
        var entity = new SavedSearch { SavedSearchId = 1, Name = "X", OwnerUserName = "abir", FilterJson = "{}" };
        _savedSearchRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _savedSearchRepositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetVisibleLookupAsync_ShouldMapIsOwnerRelativeToCurrentUser()
    {
        // Arrange
        var own = new SavedSearch { SavedSearchId = 1, Name = "Mine", OwnerUserName = "abir", IsShared = false, FilterJson = "{}" };
        var sharedByOther = new SavedSearch { SavedSearchId = 2, Name = "Theirs", OwnerUserName = "sadia", IsShared = true, FilterJson = "{}" };
        _savedSearchRepositoryMock.Setup(r => r.GetVisibleAsync("abir")).ReturnsAsync(new List<SavedSearch> { own, sharedByOther });

        // Act
        var result = await _service.GetVisibleLookupAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Single(r => r.SavedSearchId == 1).IsOwner.Should().BeTrue();
        result.Single(r => r.SavedSearchId == 2).IsOwner.Should().BeFalse();
    }
}
