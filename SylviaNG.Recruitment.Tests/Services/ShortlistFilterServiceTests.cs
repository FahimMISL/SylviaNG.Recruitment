using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ShortlistFilters.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ShortlistFilterServiceTests
{
    private readonly Mock<IShortlistFilterRepository> _shortlistFilterRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ShortlistFilterService _service;

    public ShortlistFilterServiceTests()
    {
        _shortlistFilterRepositoryMock = new Mock<IShortlistFilterRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ShortlistFilterService(_shortlistFilterRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ShortlistFilterCreateRequest CreateRequest(string name = "Senior Engineers")
    {
        return new ShortlistFilterCreateRequest
        {
            Name = name,
            Description = "Minimum bar for senior roles",
            CombineWith = FilterCombinatorEnum.And,
            Criteria = new List<ShortlistFilterCriterionRequest>
            {
                new() { CriterionType = CriterionTypeEnum.MinExperienceYears, MinExperienceYears = 5, DisplayOrder = 0 }
            }
        };
    }

    [Fact]
    public async Task CreateAsync_WithUniqueName_ShouldSaveAndReturnId()
    {
        // Arrange
        _shortlistFilterRepositoryMock.Setup(r => r.ExistsByNameAsync("Senior Engineers", null)).ReturnsAsync(false);

        ShortlistFilter? saved = null;
        _shortlistFilterRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ShortlistFilter>()))
            .Callback<ShortlistFilter>(f => { f.ShortlistFilterId = 10; saved = f; })
            .Returns(Task.CompletedTask);

        // Act
        var id = await _service.CreateAsync(CreateRequest());

        // Assert
        id.Should().Be(10);
        saved.Should().NotBeNull();
        saved!.Criteria.Should().ContainSingle();
        saved.Criteria.Single().DisplayOrder.Should().Be(0);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        // Arrange
        _shortlistFilterRepositoryMock.Setup(r => r.ExistsByNameAsync("Senior Engineers", null)).ReturnsAsync(true);

        // Act
        var act = () => _service.CreateAsync(CreateRequest());

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _shortlistFilterRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ShortlistFilter>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _shortlistFilterRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(99)).ReturnsAsync((ShortlistFilter?)null);

        // Act
        var act = () => _service.UpdateAsync(99, new ShortlistFilterUpdateRequest { Name = "X", Criteria = new() });

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceCriteriaCollectionWholesale()
    {
        // Arrange
        var entity = new ShortlistFilter
        {
            ShortlistFilterId = 1,
            Name = "Old Name",
            Criteria = new List<ShortlistFilterCriterion>
            {
                new() { ShortlistFilterCriterionId = 1, CriterionType = CriterionTypeEnum.District, RequiredDistrict = "Dhaka", DisplayOrder = 0 }
            }
        };
        _shortlistFilterRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(entity);
        _shortlistFilterRepositoryMock.Setup(r => r.ExistsByNameAsync("New Name", 1)).ReturnsAsync(false);

        var request = new ShortlistFilterUpdateRequest
        {
            Name = "New Name",
            CombineWith = FilterCombinatorEnum.Or,
            Criteria = new List<ShortlistFilterCriterionRequest>
            {
                new() { CriterionType = CriterionTypeEnum.AgeRange, MinAge = 25, MaxAge = 40, DisplayOrder = 0 },
                new() { CriterionType = CriterionTypeEnum.RequiredSkills, RequiredSkillNames = "C#,SQL", DisplayOrder = 1 }
            }
        };

        // Act
        await _service.UpdateAsync(1, request);

        // Assert
        entity.Name.Should().Be("New Name");
        entity.CombineWith.Should().Be(FilterCombinatorEnum.Or);
        entity.Criteria.Should().HaveCount(2);
        entity.Criteria.Should().NotContain(c => c.CriterionType == CriterionTypeEnum.District);
        entity.Criteria.Select(c => c.DisplayOrder).Should().BeEquivalentTo(new[] { 0, 1 });
    }

    [Fact]
    public async Task DeleteAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        // Arrange
        _shortlistFilterRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ShortlistFilter?)null);

        // Act
        var act = () => _service.DeleteAsync(99);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithKnownId_ShouldDelete()
    {
        // Arrange
        var entity = new ShortlistFilter { ShortlistFilterId = 1, Name = "X" };
        _shortlistFilterRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _shortlistFilterRepositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
