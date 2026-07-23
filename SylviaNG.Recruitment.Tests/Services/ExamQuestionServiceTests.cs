using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamQuestions.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamQuestionServiceTests
{
    private readonly Mock<IExamQuestionRepository> _examQuestionRepositoryMock;
    private readonly Mock<IQuestionGroupRepository> _questionGroupRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamQuestionService _service;

    public ExamQuestionServiceTests()
    {
        _examQuestionRepositoryMock = new Mock<IExamQuestionRepository>();
        _questionGroupRepositoryMock = new Mock<IQuestionGroupRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamQuestionService(_examQuestionRepositoryMock.Object, _questionGroupRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    private static ExamQuestionCreateRequest CreateRequest() => new()
    {
        QuestionGroupId = 1,
        QuestionText = "What is 2 + 2?",
        QuestionType = QuestionTypeEnum.McqSingle,
        DifficultyLevel = DifficultyLevelEnum.Easy,
        Marks = 5,
        Options = new List<ExamQuestionOptionRequest>
        {
            new() { OptionText = "3", IsCorrect = false, DisplayOrder = 0 },
            new() { OptionText = "4", IsCorrect = true, DisplayOrder = 1 }
        }
    };

    [Fact]
    public async Task CreateAsync_WithKnownQuestionGroup_ShouldSaveAndReturnId()
    {
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new QuestionGroup { QuestionGroupId = 1, Name = "Math" });

        ExamQuestion? saved = null;
        _examQuestionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamQuestion>()))
            .Callback<ExamQuestion>(q => { q.ExamQuestionId = 42; saved = q; })
            .Returns(Task.CompletedTask);

        var id = await _service.CreateAsync(CreateRequest());

        id.Should().Be(42);
        saved.Should().NotBeNull();
        saved!.Options.Should().HaveCount(2);
        saved.Options.Select(o => o.DisplayOrder).Should().BeEquivalentTo(new[] { 0, 1 });
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownQuestionGroupId_ShouldThrowNotFoundException()
    {
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((QuestionGroup?)null);

        var act = () => _service.CreateAsync(CreateRequest());

        await act.Should().ThrowAsync<NotFoundException>();
        _examQuestionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamQuestion>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _examQuestionRepositoryMock.Setup(r => r.GetByIdWithOptionsAsync(99)).ReturnsAsync((ExamQuestion?)null);

        var act = () => _service.UpdateAsync(99, new ExamQuestionUpdateRequest { QuestionGroupId = 1, QuestionText = "X", Options = new() });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceOptionsCollectionWholesale()
    {
        var entity = new ExamQuestion
        {
            ExamQuestionId = 1,
            QuestionGroupId = 1,
            QuestionText = "Old text",
            QuestionType = QuestionTypeEnum.McqSingle,
            DifficultyLevel = DifficultyLevelEnum.Easy,
            Marks = 5,
            Options = new List<ExamQuestionOption>
            {
                new() { ExamQuestionOptionId = 1, OptionText = "Old option A", IsCorrect = true, DisplayOrder = 0 },
                new() { ExamQuestionOptionId = 2, OptionText = "Old option B", IsCorrect = false, DisplayOrder = 1 }
            }
        };
        _examQuestionRepositoryMock.Setup(r => r.GetByIdWithOptionsAsync(1)).ReturnsAsync(entity);
        _questionGroupRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new QuestionGroup { QuestionGroupId = 1, Name = "Math" });

        var request = new ExamQuestionUpdateRequest
        {
            QuestionGroupId = 1,
            QuestionText = "New text",
            QuestionType = QuestionTypeEnum.McqSingle,
            DifficultyLevel = DifficultyLevelEnum.Medium,
            Marks = 10,
            Options = new List<ExamQuestionOptionRequest>
            {
                new() { OptionText = "New option A", IsCorrect = true, DisplayOrder = 0 }
            }
        };

        await _service.UpdateAsync(1, request);

        entity.QuestionText.Should().Be("New text");
        entity.Options.Should().ContainSingle();
        entity.Options.Single().OptionText.Should().Be("New option A");
        _examQuestionRepositoryMock.Verify(r => r.Update(entity), Times.Once);
    }

    [Fact]
    public async Task SetActiveStatusAsync_WithUnknownId_ShouldThrowNotFoundException()
    {
        _examQuestionRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ExamQuestion?)null);

        var act = () => _service.SetActiveStatusAsync(99, false);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
