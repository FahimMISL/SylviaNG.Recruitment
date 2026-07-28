using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Scorecards.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ScorecardServiceTests
{
    private readonly Mock<IScorecardRepository> _scorecardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ScorecardService _service;

    public ScorecardServiceTests()
    {
        _scorecardRepositoryMock = new Mock<IScorecardRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ScorecardService(_scorecardRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ScorecardCreateRequest CreateRequest(string name = "Technical Panel Scorecard") => new()
    {
        Name = name,
        Description = "Standard technical interview scorecard",
        Criteria = new List<ScorecardCriterionRequest>
        {
            new() { Name = "Problem Solving", Weight = 50, MaxScore = 10, DisplayOrder = 1 },
            new() { Name = "Communication", Weight = 50, MaxScore = 10, DisplayOrder = 0 },
        },
    };

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        _scorecardRepositoryMock.Setup(r => r.ExistsByNameAsync("Technical Panel Scorecard", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _scorecardRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Scorecard>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldSaveAndNormalizeDisplayOrder()
    {
        _scorecardRepositoryMock.Setup(r => r.ExistsByNameAsync("Technical Panel Scorecard", null)).ReturnsAsync(false);

        Scorecard? saved = null;
        _scorecardRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Scorecard>()))
            .Callback<Scorecard>(s => { s.ScorecardId = 9; saved = s; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(CreateRequest());

        id.Should().Be(9);
        saved.Should().NotBeNull();
        saved!.Criteria.Should().HaveCount(2);
        // Ordered by requested DisplayOrder, then renumbered 0,1,...
        saved.Criteria.Select(c => c.Name).Should().ContainInOrder("Communication", "Problem Solving");
        saved.Criteria.Select(c => c.DisplayOrder).Should().ContainInOrder(0, 1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(99)).ReturnsAsync((Scorecard?)null);

        var act = () => _service.UpdateAsync(99, new ScorecardUpdateRequest { Name = "X", Criteria = new List<ScorecardCriterionRequest>() });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceCriteriaWholesale()
    {
        var entity = new Scorecard
        {
            ScorecardId = 1,
            Name = "Old Name",
            Criteria = new List<ScorecardCriterion> { new() { ScorecardCriterionId = 1, Name = "Old Criterion", Weight = 100, MaxScore = 5, DisplayOrder = 0 } },
        };
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(entity);
        _scorecardRepositoryMock.Setup(r => r.ExistsByNameAsync("New Name", 1)).ReturnsAsync(false);

        var request = new ScorecardUpdateRequest
        {
            Name = "New Name",
            Criteria = new List<ScorecardCriterionRequest> { new() { Name = "New Criterion", Weight = 100, MaxScore = 10, DisplayOrder = 0 } },
        };

        await _service.UpdateAsync(1, request);

        entity.Name.Should().Be("New Name");
        entity.Criteria.Should().ContainSingle(c => c.Name == "New Criterion");
        _scorecardRepositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _scorecardRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Scorecard?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
