using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.InterviewEvaluations.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class InterviewEvaluationServiceTests
{
    private readonly Mock<IInterviewEvaluationRepository> _interviewEvaluationRepositoryMock;
    private readonly Mock<IInterviewRepository> _interviewRepositoryMock;
    private readonly Mock<IScorecardRepository> _scorecardRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InterviewEvaluationService _service;

    public InterviewEvaluationServiceTests()
    {
        _interviewEvaluationRepositoryMock = new Mock<IInterviewEvaluationRepository>();
        _interviewRepositoryMock = new Mock<IInterviewRepository>();
        _scorecardRepositoryMock = new Mock<IScorecardRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new InterviewEvaluationService(
            _interviewEvaluationRepositoryMock.Object,
            _interviewRepositoryMock.Object,
            _scorecardRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static Interview InterviewWithPanelist(long interviewId, long employeeId, InterviewStatusEnum status = InterviewStatusEnum.Scheduled) => new()
    {
        InterviewId = interviewId,
        Status = status,
        PanelMembers = new List<InterviewPanelMember> { new() { InterviewId = interviewId, EmployeeId = employeeId } },
    };

    private static Scorecard ActiveScorecard(long scorecardId, bool isActive = true) => new()
    {
        ScorecardId = scorecardId,
        Name = "Technical Panel",
        IsActive = isActive,
        Criteria = new List<ScorecardCriterion>
        {
            new() { ScorecardCriterionId = 1, ScorecardId = scorecardId, Name = "Problem Solving", Weight = 60, MaxScore = 10, DisplayOrder = 0 },
            new() { ScorecardCriterionId = 2, ScorecardId = scorecardId, Name = "Communication", Weight = 40, MaxScore = 10, DisplayOrder = 1 },
        },
    };

    private static InterviewEvaluationSubmitRequest ValidRequest(long employeeId = 5, long scorecardId = 1) => new()
    {
        EmployeeId = employeeId,
        ScorecardId = scorecardId,
        Scores = new List<InterviewEvaluationScoreRequest>
        {
            new() { ScorecardCriterionId = 1, Score = 8 },
            new() { ScorecardCriterionId = 2, Score = 7 },
        },
        OverallComments = "Strong candidate",
    };

    [Fact]
    public async Task SubmitAsync_InterviewNotFound_ShouldThrowNotFoundException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync((Interview?)null);

        var act = () => _service.SubmitAsync(1, ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SubmitAsync_CancelledInterview_ShouldThrowInvalidStatusTransitionException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(InterviewWithPanelist(1, 5, InterviewStatusEnum.Cancelled));

        var act = () => _service.SubmitAsync(1, ValidRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitAsync_EmployeeNotPanelist_ShouldThrowInvalidStatusTransitionException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));

        var act = () => _service.SubmitAsync(1, ValidRequest(employeeId: 999));

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitAsync_AlreadyEvaluatedByThisPanelist_ShouldThrowDuplicateException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));
        _interviewEvaluationRepositoryMock.Setup(r => r.ExistsByInterviewAndEmployeeAsync(1, 5, null)).ReturnsAsync(true);

        var act = () => _service.SubmitAsync(1, ValidRequest());

        await act.Should().ThrowAsync<DuplicateException>();
        _interviewEvaluationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<InterviewEvaluation>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_InactiveScorecard_ShouldThrowInvalidStatusTransitionException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));
        _interviewEvaluationRepositoryMock.Setup(r => r.ExistsByInterviewAndEmployeeAsync(1, 5, null)).ReturnsAsync(false);
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(ActiveScorecard(1, isActive: false));

        var act = () => _service.SubmitAsync(1, ValidRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitAsync_MissingCriterionScore_ShouldThrowInvalidStatusTransitionException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));
        _interviewEvaluationRepositoryMock.Setup(r => r.ExistsByInterviewAndEmployeeAsync(1, 5, null)).ReturnsAsync(false);
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(ActiveScorecard(1));

        var request = ValidRequest();
        request.Scores.RemoveAt(1); // only 1 of 2 criteria scored

        var act = () => _service.SubmitAsync(1, request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitAsync_ScoreAboveMaxScore_ShouldThrowInvalidStatusTransitionException()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));
        _interviewEvaluationRepositoryMock.Setup(r => r.ExistsByInterviewAndEmployeeAsync(1, 5, null)).ReturnsAsync(false);
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(ActiveScorecard(1));

        var request = ValidRequest();
        request.Scores[0].Score = 15; // MaxScore is 10

        var act = () => _service.SubmitAsync(1, request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task SubmitAsync_ValidRequest_ShouldSaveAndReturnId()
    {
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(InterviewWithPanelist(1, 5));
        _interviewEvaluationRepositoryMock.Setup(r => r.ExistsByInterviewAndEmployeeAsync(1, 5, null)).ReturnsAsync(false);
        _scorecardRepositoryMock.Setup(r => r.GetByIdWithCriteriaAsync(1)).ReturnsAsync(ActiveScorecard(1));
        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("hr.abir");

        InterviewEvaluation? saved = null;
        _interviewEvaluationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<InterviewEvaluation>()))
            .Callback<InterviewEvaluation>(e => { e.InterviewEvaluationId = 42; saved = e; })
            .Returns(Task.CompletedTask);

        var id = await _service.SubmitAsync(1, ValidRequest());

        id.Should().Be(42);
        saved.Should().NotBeNull();
        saved!.EmployeeId.Should().Be(5);
        saved.ScorecardId.Should().Be(1);
        saved.Scores.Should().HaveCount(2);
        saved.SubmittedByUserName.Should().Be("hr.abir");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
