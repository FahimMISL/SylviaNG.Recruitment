using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Exams.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamServiceTests
{
    private readonly Mock<IExamRepository> _examRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IExamVenueRepository> _examVenueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamService _service;

    public ExamServiceTests()
    {
        _examRepositoryMock = new Mock<IExamRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _examVenueRepositoryMock = new Mock<IExamVenueRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamService(
            _examRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _examVenueRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private static ExamCreateRequest CreateRequest(
        long jobPostingId = 1,
        ExamTypeEnum examType = ExamTypeEnum.InPerson,
        long? examVenueId = 10,
        long? questionGroupId = null) => new()
    {
        JobPostingId = jobPostingId,
        Title = "Written Test",
        ScheduledStartAt = DateTime.UtcNow.AddDays(3),
        DurationMinutes = 60,
        TotalMarks = 100,
        PassMarks = 40,
        ExamType = examType,
        ExamVenueId = examVenueId,
        QuestionGroupId = questionGroupId,
    };

    private void SetupKnownJobPosting(long jobPostingId = 1)
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(jobPostingId)).ReturnsAsync(new JobPosting { JobPostingId = jobPostingId });
    }

    [Fact]
    public async Task CreateAsync_InPersonWithoutExamVenueId_ShouldThrowInvalidStatusTransitionException()
    {
        SetupKnownJobPosting();
        var request = CreateRequest(examType: ExamTypeEnum.InPerson, examVenueId: null);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_OnlineWithoutQuestionGroupId_ShouldThrowInvalidStatusTransitionException()
    {
        SetupKnownJobPosting();
        var request = CreateRequest(examType: ExamTypeEnum.Online, examVenueId: null, questionGroupId: null);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UnknownJobPosting_ShouldThrowNotFoundException()
    {
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((JobPosting?)null);
        var request = CreateRequest();

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<NotFoundException>();
        _examRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_UnknownExamVenue_ShouldThrowNotFoundException()
    {
        SetupKnownJobPosting();
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((ExamVenue?)null);
        var request = CreateRequest(examType: ExamTypeEnum.InPerson, examVenueId: 10);

        var act = () => _service.CreateAsync(request);

        await act.Should().ThrowAsync<NotFoundException>();
        _examRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Exam>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidInPersonRequest_ShouldSaveAndReturnNewId()
    {
        SetupKnownJobPosting();
        _examVenueRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new ExamVenue { ExamVenueId = 10 });

        Exam? saved = null;
        _examRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Exam>()))
            .Callback<Exam>(exam => { exam.ExamId = 55; saved = exam; })
            .Returns(Task.CompletedTask);

        var request = CreateRequest(examType: ExamTypeEnum.InPerson, examVenueId: 10);

        var id = await _service.CreateAsync(request);

        id.Should().Be(55);
        saved.Should().NotBeNull();
        saved!.JobPostingId.Should().Be(1);
        saved.ExamVenueId.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidOnlineRequest_ShouldSaveAndReturnNewId()
    {
        SetupKnownJobPosting();

        Exam? saved = null;
        _examRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Exam>()))
            .Callback<Exam>(exam => { exam.ExamId = 56; saved = exam; })
            .Returns(Task.CompletedTask);

        var request = CreateRequest(examType: ExamTypeEnum.Online, examVenueId: null, questionGroupId: 5);

        var id = await _service.CreateAsync(request);

        id.Should().Be(56);
        saved.Should().NotBeNull();
        saved!.QuestionGroupId.Should().Be(5);
        _examVenueRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
