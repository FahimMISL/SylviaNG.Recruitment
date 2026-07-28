using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.Interviews.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class InterviewServiceTests
{
    private readonly Mock<IInterviewRepository> _interviewRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IInterviewVenueRepository> _interviewVenueRepositoryMock;
    private readonly Mock<IInterviewRoomRepository> _interviewRoomRepositoryMock;
    private readonly Mock<IInterviewRoundConfigRepository> _interviewRoundConfigRepositoryMock;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly Mock<IInterviewNotificationService> _interviewNotificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly InterviewService _service;

    public InterviewServiceTests()
    {
        _interviewRepositoryMock = new Mock<IInterviewRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _interviewVenueRepositoryMock = new Mock<IInterviewVenueRepository>();
        _interviewRoomRepositoryMock = new Mock<IInterviewRoomRepository>();
        _interviewRoundConfigRepositoryMock = new Mock<IInterviewRoundConfigRepository>();
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _interviewNotificationServiceMock = new Mock<IInterviewNotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new InterviewService(
            _interviewRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _interviewVenueRepositoryMock.Object,
            _interviewRoomRepositoryMock.Object,
            _interviewRoundConfigRepositoryMock.Object,
            _employeeRepositoryMock.Object,
            _interviewNotificationServiceMock.Object,
            _unitOfWorkMock.Object,
            new Mock<ILogger<InterviewService>>().Object);
    }

    private static JobApplication ApplicationFor(long jobApplicationId) => new()
    {
        JobApplicationId = jobApplicationId,
        JobPostingId = 100,
        CandidateName = "Candidate " + jobApplicationId,
    };

    private static InterviewScheduleRequest InPersonRequest(long jobApplicationId = 5, long roomId = 20, List<long>? panelistIds = null) => new()
    {
        JobApplicationId = jobApplicationId,
        InterviewType = InterviewTypeEnum.InPerson,
        InterviewRoomId = roomId,
        ScheduledStartAt = new DateTime(2026, 8, 1, 10, 0, 0),
        ScheduledEndAt = new DateTime(2026, 8, 1, 10, 30, 0),
        PanelistEmployeeIds = panelistIds ?? new List<long> { 1 },
    };

    [Fact]
    public async Task ScheduleAsync_InPersonWithoutRoomId_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));

        var request = InPersonRequest();
        request.InterviewRoomId = null;

        var act = () => _service.ScheduleAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_VirtualWithoutMeetingLink_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));

        var request = InPersonRequest();
        request.InterviewType = InterviewTypeEnum.Virtual;
        request.InterviewRoomId = null;
        request.MeetingLink = null;

        var act = () => _service.ScheduleAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task ScheduleAsync_UnknownPanelistEmployee_ShouldThrowNotFoundException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new InterviewRoom { InterviewRoomId = 20, InterviewVenueId = 2, Capacity = 5 });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        var act = () => _service.ScheduleAsync(InPersonRequest());

        await act.Should().ThrowAsync<NotFoundException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_RoomAtCapacity_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new InterviewRoom { InterviewRoomId = 20, InterviewVenueId = 2, Capacity = 1 });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1 });
        _interviewRepositoryMock.Setup(r => r.GetOverlappingCountByRoomIdAsync(20, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(1);
        _interviewRepositoryMock.Setup(r => r.GetConflictingPanelistEmployeeIdsAsync(It.IsAny<List<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(new List<long>());

        var act = () => _service.ScheduleAsync(InPersonRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_PanelistAlreadyBooked_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new InterviewRoom { InterviewRoomId = 20, InterviewVenueId = 2, Capacity = 5 });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1 });
        _interviewRepositoryMock.Setup(r => r.GetOverlappingCountByRoomIdAsync(20, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(0);
        _interviewRepositoryMock.Setup(r => r.GetConflictingPanelistEmployeeIdsAsync(It.IsAny<List<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(new List<long> { 1 });

        var act = () => _service.ScheduleAsync(InPersonRequest());

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_ValidInPersonRequest_ShouldDeriveVenueFromRoomAndNotify()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new InterviewRoom { InterviewRoomId = 20, InterviewVenueId = 2, Capacity = 5 });
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new InterviewVenue { InterviewVenueId = 2, VenueName = "Main Campus" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1 });
        _interviewRepositoryMock.Setup(r => r.GetOverlappingCountByRoomIdAsync(20, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(0);
        _interviewRepositoryMock.Setup(r => r.GetConflictingPanelistEmployeeIdsAsync(It.IsAny<List<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(new List<long>());

        Interview? saved = null;
        _interviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Interview>()))
            .Callback<Interview>(i => { i.InterviewId = 42; saved = i; })
            .Returns(Task.CompletedTask);

        var id = await _service.ScheduleAsync(InPersonRequest());

        id.Should().Be(42);
        saved.Should().NotBeNull();
        saved!.InterviewVenueId.Should().Be(2);
        saved.InterviewRoomId.Should().Be(20);
        saved.PanelMembers.Should().ContainSingle(p => p.EmployeeId == 1);
        _interviewNotificationServiceMock.Verify(n => n.NotifyScheduledAsync(It.IsAny<Interview>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RescheduleAsync_CancelledInterview_ShouldThrowInvalidStatusTransitionException()
    {
        var interview = new Interview
        {
            InterviewId = 1,
            InterviewType = InterviewTypeEnum.Virtual,
            MeetingLink = "https://meet",
            Status = InterviewStatusEnum.Cancelled,
        };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);

        var act = () => _service.RescheduleAsync(1, new InterviewRescheduleRequest
        {
            ScheduledStartAt = DateTime.UtcNow,
            ScheduledEndAt = DateTime.UtcNow.AddMinutes(30),
        });

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task RescheduleAsync_ValidRequest_ShouldUpdateTimesAndSetStatusRescheduled()
    {
        var interview = new Interview
        {
            InterviewId = 1,
            JobApplication = ApplicationFor(5),
            InterviewType = InterviewTypeEnum.Virtual,
            MeetingLink = "https://meet",
            Status = InterviewStatusEnum.Scheduled,
            ScheduledStartAt = new DateTime(2026, 8, 1, 10, 0, 0),
            ScheduledEndAt = new DateTime(2026, 8, 1, 10, 30, 0),
        };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);
        _interviewRepositoryMock.Setup(r => r.GetConflictingPanelistEmployeeIdsAsync(It.IsAny<List<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1)).ReturnsAsync(new List<long>());

        var newStart = new DateTime(2026, 8, 2, 11, 0, 0);
        var newEnd = new DateTime(2026, 8, 2, 11, 30, 0);

        await _service.RescheduleAsync(1, new InterviewRescheduleRequest { ScheduledStartAt = newStart, ScheduledEndAt = newEnd });

        interview.ScheduledStartAt.Should().Be(newStart);
        interview.ScheduledEndAt.Should().Be(newEnd);
        interview.Status.Should().Be(InterviewStatusEnum.Rescheduled);
        _interviewNotificationServiceMock.Verify(n => n.NotifyRescheduledAsync(interview), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_AlreadyCancelled_ShouldThrowInvalidStatusTransitionException()
    {
        var interview = new Interview { InterviewId = 1, Status = InterviewStatusEnum.Cancelled };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);

        var act = () => _service.CancelAsync(1, new InterviewCancelRequest { CancellationReason = "Test" });

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task CancelAsync_ValidRequest_ShouldSetStatusCancelledAndNotify()
    {
        var interview = new Interview { InterviewId = 1, JobApplication = ApplicationFor(5), Status = InterviewStatusEnum.Scheduled };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);

        await _service.CancelAsync(1, new InterviewCancelRequest { CancellationReason = "Candidate withdrew" });

        interview.Status.Should().Be(InterviewStatusEnum.Cancelled);
        interview.CancellationReason.Should().Be("Candidate withdrew");
        _interviewNotificationServiceMock.Verify(n => n.NotifyCancelledAsync(interview), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task BulkCancelAsync_SkipsAlreadyCancelledButCancelsRest()
    {
        var interviews = new List<Interview>
        {
            new() { InterviewId = 1, JobApplication = ApplicationFor(5), Status = InterviewStatusEnum.Scheduled },
            new() { InterviewId = 2, JobApplication = ApplicationFor(6), Status = InterviewStatusEnum.Cancelled, CancellationReason = "Old reason" },
        };
        _interviewRepositoryMock.Setup(r => r.GetByIdsWithDetailsAsync(It.IsAny<List<long>>())).ReturnsAsync(interviews);

        await _service.BulkCancelAsync(new InterviewBulkCancelRequest { InterviewIds = new List<long> { 1, 2 }, CancellationReason = "Posting closed" });

        interviews[0].Status.Should().Be(InterviewStatusEnum.Cancelled);
        interviews[0].CancellationReason.Should().Be("Posting closed");
        interviews[1].CancellationReason.Should().Be("Old reason");
        _interviewNotificationServiceMock.Verify(n => n.NotifyCancelledAsync(It.IsAny<Interview>()), Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    // ── US-070: round config gating ─────────────────────────────────

    private void SetupHappyPathForRoundConfigTests()
    {
        _interviewRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(new InterviewRoom { InterviewRoomId = 20, InterviewVenueId = 2, Capacity = 5 });
        _interviewVenueRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new InterviewVenue { InterviewVenueId = 2, VenueName = "Main Campus" });
        _employeeRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee { EmployeeId = 1 });
        _interviewRepositoryMock.Setup(r => r.GetOverlappingCountByRoomIdAsync(20, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(0);
        _interviewRepositoryMock.Setup(r => r.GetConflictingPanelistEmployeeIdsAsync(It.IsAny<List<long>>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(new List<long>());
        _interviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Interview>()))
            .Callback<Interview>(i => i.InterviewId = 42)
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task ScheduleAsync_RoundConfigWrongJobPosting_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 999, Sequence = 1, Name = "Technical Round 1" });

        var request = InPersonRequest();
        request.InterviewRoundConfigId = 1;

        var act = () => _service.ScheduleAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_RoundConfigSequenceOne_ShouldSetRoundAndConfigIdWithNoGateCheck()
    {
        SetupHappyPathForRoundConfigTests();
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 100, Sequence = 1, Name = "Technical Round 1" });

        Interview? saved = null;
        _interviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Interview>()))
            .Callback<Interview>(i => { i.InterviewId = 42; saved = i; })
            .Returns(Task.CompletedTask);

        var request = InPersonRequest();
        request.InterviewRoundConfigId = 1;

        await _service.ScheduleAsync(request);

        saved!.Round.Should().Be(1);
        saved.InterviewRoundConfigId.Should().Be(1);
        _interviewRepositoryMock.Verify(r => r.ExistsPassedForRoundConfigAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_RoundConfigSequenceTwo_PriorRoundNotPassed_ShouldThrowInvalidStatusTransitionException()
    {
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(2))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 2, JobPostingId = 100, Sequence = 2, Name = "HR Round" });
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByJobPostingAndSequenceAsync(100, 1))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 100, Sequence = 1, Name = "Technical Round 1" });
        _interviewRepositoryMock.Setup(r => r.ExistsPassedForRoundConfigAsync(5, 1)).ReturnsAsync(false);

        var request = InPersonRequest();
        request.InterviewRoundConfigId = 2;

        var act = () => _service.ScheduleAsync(request);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _interviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Interview>()), Times.Never);
    }

    [Fact]
    public async Task ScheduleAsync_RoundConfigSequenceTwo_PriorRoundPassed_ShouldSucceed()
    {
        SetupHappyPathForRoundConfigTests();
        _jobApplicationRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(ApplicationFor(5));
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(2))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 2, JobPostingId = 100, Sequence = 2, Name = "HR Round" });
        _interviewRoundConfigRepositoryMock.Setup(r => r.GetByJobPostingAndSequenceAsync(100, 1))
            .ReturnsAsync(new InterviewRoundConfig { InterviewRoundConfigId = 1, JobPostingId = 100, Sequence = 1, Name = "Technical Round 1" });
        _interviewRepositoryMock.Setup(r => r.ExistsPassedForRoundConfigAsync(5, 1)).ReturnsAsync(true);

        Interview? saved = null;
        _interviewRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Interview>()))
            .Callback<Interview>(i => { i.InterviewId = 42; saved = i; })
            .Returns(Task.CompletedTask);

        var request = InPersonRequest();
        request.InterviewRoundConfigId = 2;

        await _service.ScheduleAsync(request);

        saved!.Round.Should().Be(2);
        saved.InterviewRoundConfigId.Should().Be(2);
    }

    [Fact]
    public async Task MarkResultAsync_CancelledInterview_ShouldThrowInvalidStatusTransitionException()
    {
        var interview = new Interview { InterviewId = 1, Status = InterviewStatusEnum.Cancelled };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);

        var act = () => _service.MarkResultAsync(1, new InterviewMarkResultRequest { Result = InterviewResultEnum.Passed });

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task MarkResultAsync_ValidRequest_ShouldSetResultAndStatusCompleted()
    {
        var interview = new Interview { InterviewId = 1, Status = InterviewStatusEnum.Scheduled };
        _interviewRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(interview);

        await _service.MarkResultAsync(1, new InterviewMarkResultRequest { Result = InterviewResultEnum.Passed });

        interview.Result.Should().Be(InterviewResultEnum.Passed);
        interview.Status.Should().Be(InterviewStatusEnum.Completed);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
