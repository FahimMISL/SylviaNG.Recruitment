using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.QuestionGroups.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class QuestionGroupServiceTests
{
    private readonly Mock<IQuestionGroupRepository> _questionGroupRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly QuestionGroupService _service;

    public QuestionGroupServiceTests()
    {
        _questionGroupRepositoryMock = new Mock<IQuestionGroupRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new QuestionGroupService(_questionGroupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static QuestionGroupCreateRequest CreateRequest(string name = "Aptitude") => new()
    {
        Name = name,
        Description = "General aptitude questions"
    };

    [Fact]
    public async Task CreateAsync_WithUniqueName_ShouldSaveAndReturnId()
    {
        _questionGroupRepositoryMock.Setup(r => r.ExistsByNameAsync("Aptitude", null)).ReturnsAsync(false);

        QuestionGroup? saved = null;
        _questionGroupRepositoryMock.Setup(r => r.AddAsync(It.IsAny<QuestionGroup>()))
            .Callback<QuestionGroup>(g => { g.QuestionGroupId = 5; saved = g; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(CreateRequest());

        id.Should().Be(5);
        saved.Should().NotBeNull();
        saved!.IsActive.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateName_ShouldThrowDuplicateException()
    {
        _questionGroupRepositoryMock.Setup(r => r.ExistsByNameAsync("Aptitude", null)).ReturnsAsync(true);

        var act = () => _service.CreateAsync(CreateRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _questionGroupRepositoryMock.Verify(r => r.AddAsync(It.IsAny<QuestionGroup>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((QuestionGroup?)null);

        var act = () => _service.UpdateAsync(99, new QuestionGroupUpdateRequest { Name = "X" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((QuestionGroup?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SetActiveStatusAsync_Deactivate_ShouldPersistIsActiveFalse()
    {
        var entity = new QuestionGroup { QuestionGroupId = 1, Name = "Aptitude", IsActive = true };
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        await _service.SetActiveStatusAsync(1, false);

        entity.IsActive.Should().BeFalse();
        _questionGroupRepositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
