using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExamTaking.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamTakingServiceTests
{
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<IExamQuestionRepository> _examQuestionRepositoryMock;
    private readonly Mock<IExamAnswerRepository> _examAnswerRepositoryMock;
    private readonly Mock<ICurrentCandidateService> _currentCandidateServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamTakingService _service;

    public ExamTakingServiceTests()
    {
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _examQuestionRepositoryMock = new Mock<IExamQuestionRepository>();
        _examAnswerRepositoryMock = new Mock<IExamAnswerRepository>();
        _currentCandidateServiceMock = new Mock<ICurrentCandidateService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _currentCandidateServiceMock.Setup(s => s.GetOrCreateCurrentProfileIdAsync()).ReturnsAsync(5);

        _service = new ExamTakingService(
            _examEnrollmentRepositoryMock.Object,
            _examQuestionRepositoryMock.Object,
            _examAnswerRepositoryMock.Object,
            _currentCandidateServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static ExamEnrollment EnrollmentFor(
        long candidateProfileId,
        ExamTypeEnum examType = ExamTypeEnum.Online,
        DateTime? scheduledStartAt = null,
        DateTime? startedAt = null,
        DateTime? submittedAt = null,
        bool showResultsToCandidate = true,
        decimal passMarks = 40) => new()
    {
        ExamEnrollmentId = 1,
        ExamId = 10,
        JobApplicationId = 50,
        StartedAt = startedAt,
        SubmittedAt = submittedAt,
        JobApplication = new JobApplication { JobApplicationId = 50, CandidateProfileId = candidateProfileId, CandidateName = "Candidate" },
        Exam = new Exam
        {
            ExamId = 10,
            ExamType = examType,
            ScheduledStartAt = scheduledStartAt ?? DateTime.UtcNow.AddHours(-1),
            DurationMinutes = 60,
            QuestionGroupId = 20,
            TotalMarks = 100,
            PassMarks = passMarks,
            ShowResultsToCandidate = showResultsToCandidate,
        },
    };

    private static ExamQuestion McqSingleQuestion(long id, decimal marks, long correctOptionId) => new()
    {
        ExamQuestionId = id,
        QuestionGroupId = 20,
        QuestionType = QuestionTypeEnum.McqSingle,
        QuestionText = "2+2?",
        Marks = marks,
        IsActive = true,
        Options = new List<ExamQuestionOption>
        {
            new() { ExamQuestionOptionId = correctOptionId, OptionText = "4", IsCorrect = true },
            new() { ExamQuestionOptionId = correctOptionId + 1, OptionText = "5", IsCorrect = false },
        },
    };

    [Fact]
    public async Task StartExamAsync_NotOwnedByCurrentCandidate_ShouldThrowForbiddenException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 999);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.StartExamAsync(1);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task StartExamAsync_InPersonExam_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, examType: ExamTypeEnum.InPerson);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.StartExamAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task StartExamAsync_AlreadySubmitted_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, submittedAt: DateTime.UtcNow);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.StartExamAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task StartExamAsync_BeforeScheduledStartTime_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, scheduledStartAt: DateTime.UtcNow.AddHours(1));
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.StartExamAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task StartExamAsync_FirstCall_ShouldSetStartedAtAndReturnPaperWithoutAnswerKey()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20))
            .ReturnsAsync(new List<ExamQuestion> { McqSingleQuestion(100, 5, 200) });

        var paper = await _service.StartExamAsync(1);

        enrollment.StartedAt.Should().NotBeNull();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(enrollment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        paper.Questions.Should().ContainSingle();
        paper.Questions[0].Options.Should().HaveCount(2);
        paper.DeadlineAt.Should().Be(enrollment.StartedAt!.Value.AddMinutes(60));
    }

    [Fact]
    public async Task StartExamAsync_AlreadyStarted_ShouldNotResetStartedAt()
    {
        var originalStart = DateTime.UtcNow.AddMinutes(-10);
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: originalStart);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20)).ReturnsAsync(new List<ExamQuestion>());

        await _service.StartExamAsync(1);

        enrollment.StartedAt.Should().Be(originalStart);
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task SubmitExamAsync_NotStarted_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.SubmitExamAsync(1, new ExamSubmitRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitExamAsync_AlreadySubmitted_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5), submittedAt: DateTime.UtcNow);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.SubmitExamAsync(1, new ExamSubmitRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitExamAsync_McqSingleCorrectAnswer_ShouldAwardFullMarksAndPass()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5), passMarks: 4);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20))
            .ReturnsAsync(new List<ExamQuestion> { McqSingleQuestion(100, 5, 200) });

        var request = new ExamSubmitRequest
        {
            Answers = new List<ExamAnswerRequest>
            {
                new() { ExamQuestionId = 100, SelectedOptionIds = new List<long> { 200 } },
            },
        };

        var result = await _service.SubmitExamAsync(1, request);

        enrollment.Score.Should().Be(5);
        enrollment.IsPassed.Should().BeTrue();
        enrollment.ScoreSource.Should().Be(ScoreSourceEnum.AutoScored);
        result.ResultsVisible.Should().BeTrue();
        result.Score.Should().Be(5);
        result.IsPassed.Should().BeTrue();
        _examAnswerRepositoryMock.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<ExamAnswer>>(a => a.Count() == 1)), Times.Once);
    }

    [Fact]
    public async Task SubmitExamAsync_McqSingleWrongAnswer_ShouldAwardZeroMarks()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5));
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20))
            .ReturnsAsync(new List<ExamQuestion> { McqSingleQuestion(100, 5, 200) });

        var request = new ExamSubmitRequest
        {
            Answers = new List<ExamAnswerRequest>
            {
                new() { ExamQuestionId = 100, SelectedOptionIds = new List<long> { 201 } },
            },
        };

        var result = await _service.SubmitExamAsync(1, request);

        enrollment.Score.Should().Be(0);
        enrollment.IsPassed.Should().BeFalse();
    }

    [Fact]
    public async Task SubmitExamAsync_UnansweredQuestion_ShouldAwardZeroMarks()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5));
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20))
            .ReturnsAsync(new List<ExamQuestion> { McqSingleQuestion(100, 5, 200) });

        var result = await _service.SubmitExamAsync(1, new ExamSubmitRequest());

        enrollment.Score.Should().Be(0);
    }

    [Fact]
    public async Task SubmitExamAsync_McqMultiplePartialSelection_ShouldAwardZeroMarksAllOrNothing()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5));
        var question = new ExamQuestion
        {
            ExamQuestionId = 101,
            QuestionGroupId = 20,
            QuestionType = QuestionTypeEnum.McqMultiple,
            Marks = 10,
            IsActive = true,
            Options = new List<ExamQuestionOption>
            {
                new() { ExamQuestionOptionId = 300, IsCorrect = true },
                new() { ExamQuestionOptionId = 301, IsCorrect = true },
                new() { ExamQuestionOptionId = 302, IsCorrect = false },
            },
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20)).ReturnsAsync(new List<ExamQuestion> { question });

        var request = new ExamSubmitRequest
        {
            Answers = new List<ExamAnswerRequest>
            {
                new() { ExamQuestionId = 101, SelectedOptionIds = new List<long> { 300 } }, // only 1 of 2 correct options
            },
        };

        var result = await _service.SubmitExamAsync(1, request);

        enrollment.Score.Should().Be(0);
    }

    [Fact]
    public async Task SubmitExamAsync_SubjectiveQuestion_ShouldLeaveUngradedAndNotCountTowardScore()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5));
        var question = new ExamQuestion
        {
            ExamQuestionId = 102,
            QuestionGroupId = 20,
            QuestionType = QuestionTypeEnum.Subjective,
            Marks = 20,
            IsActive = true,
            Options = new List<ExamQuestionOption>(),
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20)).ReturnsAsync(new List<ExamQuestion> { question });

        var request = new ExamSubmitRequest
        {
            Answers = new List<ExamAnswerRequest> { new() { ExamQuestionId = 102, AnswerText = "My essay answer." } },
        };

        var result = await _service.SubmitExamAsync(1, request);

        enrollment.Score.Should().Be(0);
        _examAnswerRepositoryMock.Verify(
            r => r.AddRangeAsync(It.Is<IEnumerable<ExamAnswer>>(a => a.Single().IsCorrect == null && a.Single().MarksAwarded == null && a.Single().AnswerText == "My essay answer.")),
            Times.Once);
    }

    [Fact]
    public async Task SubmitExamAsync_ResultsNotEnabledForExam_ShouldHideScoreInResponse()
    {
        var enrollment = EnrollmentFor(candidateProfileId: 5, startedAt: DateTime.UtcNow.AddMinutes(-5), showResultsToCandidate: false);
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithExamAndQuestionsAsync(1)).ReturnsAsync(enrollment);
        _examQuestionRepositoryMock.Setup(r => r.GetActiveByQuestionGroupIdAsync(20)).ReturnsAsync(new List<ExamQuestion>());

        var result = await _service.SubmitExamAsync(1, new ExamSubmitRequest());

        result.ResultsVisible.Should().BeFalse();
        result.Score.Should().BeNull();
        result.IsPassed.Should().BeNull();
        result.ReferenceNumber.Should().Be("EXM-10-1");
    }
}
