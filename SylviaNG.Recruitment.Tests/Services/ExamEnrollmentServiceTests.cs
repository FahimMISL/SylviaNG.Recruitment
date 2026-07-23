using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExamEnrollmentServiceTests
{
    private readonly Mock<IExamRepository> _examRepositoryMock;
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IExamRoomRepository> _examRoomRepositoryMock;
    private readonly Mock<IExamNotificationService> _examNotificationServiceMock;
    private readonly Mock<IJobApplicationStageProgressService> _jobApplicationStageProgressServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamEnrollmentService _service;

    public ExamEnrollmentServiceTests()
    {
        _examRepositoryMock = new Mock<IExamRepository>();
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _examRoomRepositoryMock = new Mock<IExamRoomRepository>();
        _examNotificationServiceMock = new Mock<IExamNotificationService>();
        _jobApplicationStageProgressServiceMock = new Mock<IJobApplicationStageProgressService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamEnrollmentService(
            _examRepositoryMock.Object,
            _examEnrollmentRepositoryMock.Object,
            _jobApplicationRepositoryMock.Object,
            _examRoomRepositoryMock.Object,
            _examNotificationServiceMock.Object,
            _jobApplicationStageProgressServiceMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object,
            new Mock<ILogger<ExamEnrollmentService>>().Object);
    }

    private static Exam ExamFor(long examId, long jobPostingId, long? examVenueId = null, decimal totalMarks = 100, decimal passMarks = 40) => new()
    {
        ExamId = examId,
        JobPostingId = jobPostingId,
        ExamVenueId = examVenueId,
        TotalMarks = totalMarks,
        PassMarks = passMarks,
    };

    private static JobApplication ApplicationFor(
        long jobApplicationId,
        long jobPostingId,
        ApplicationStatusEnum status = ApplicationStatusEnum.Shortlisted) => new()
    {
        JobApplicationId = jobApplicationId,
        JobPostingId = jobPostingId,
        CandidateName = "Candidate " + jobApplicationId,
        ApplicationStatus = status,
    };

    private void SetupJobApplications(params JobApplication[] applications)
    {
        _jobApplicationRepositoryMock
            .Setup(r => r.GetByIdWithIncludeAsync(It.IsAny<Expression<Func<JobApplication, bool>>>(), It.IsAny<Expression<Func<JobApplication, object>>[]>()))
            .ReturnsAsync((Expression<Func<JobApplication, bool>> predicate, Expression<Func<JobApplication, object>>[] _) =>
                applications.FirstOrDefault(predicate.Compile()));
    }

    [Fact]
    public async Task EnrollAsync_JobApplicationNotShortlisted_ShouldThrowInvalidStatusTransitionException()
    {
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ExamFor(1, 100));
        SetupJobApplications(ApplicationFor(5, 100, ApplicationStatusEnum.Applied));

        var act = () => _service.EnrollAsync(1, new List<long> { 5 });

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examEnrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task EnrollAsync_JobApplicationForDifferentJobPosting_ShouldThrowInvalidStatusTransitionException()
    {
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ExamFor(1, 100));
        SetupJobApplications(ApplicationFor(5, 999, ApplicationStatusEnum.Shortlisted));

        var act = () => _service.EnrollAsync(1, new List<long> { 5 });

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examEnrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task EnrollAsync_AlreadyEnrolledJobApplication_ShouldThrowDuplicateException()
    {
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ExamFor(1, 100));
        SetupJobApplications(ApplicationFor(5, 100, ApplicationStatusEnum.Shortlisted));
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsByExamAndJobApplicationAsync(1, 5)).ReturnsAsync(true);

        var act = () => _service.EnrollAsync(1, new List<long> { 5 });

        await act.Should().ThrowAsync<DuplicateException>();
        _examEnrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task EnrollAsync_HappyPath_ShouldCreateEnrollmentsAndNotifyOncePerEnrollment()
    {
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ExamFor(1, 100));
        SetupJobApplications(
            ApplicationFor(5, 100, ApplicationStatusEnum.Shortlisted),
            ApplicationFor(6, 100, ApplicationStatusEnum.Shortlisted));
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsByExamAndJobApplicationAsync(1, It.IsAny<long>())).ReturnsAsync(false);

        var nextId = 900L;
        _examEnrollmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamEnrollment>()))
            .Callback<ExamEnrollment>(e => e.ExamEnrollmentId = ++nextId)
            .Returns(Task.CompletedTask);

        var ids = await _service.EnrollAsync(1, new List<long> { 5, 6 });

        ids.Should().HaveCount(2);
        ids.Should().Contain(new[] { 901L, 902L });
        _examEnrollmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExamEnrollment>()), Times.Exactly(2));
        _examNotificationServiceMock.Verify(
            n => n.NotifyEnrollmentAsync(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()),
            Times.Exactly(2));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task EnrollAsync_WhenNotificationServiceThrows_ShouldStillCompleteAndReturnEnrollmentIds()
    {
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ExamFor(1, 100));
        SetupJobApplications(ApplicationFor(5, 100, ApplicationStatusEnum.Shortlisted));
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsByExamAndJobApplicationAsync(1, 5)).ReturnsAsync(false);
        _examEnrollmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExamEnrollment>()))
            .Callback<ExamEnrollment>(e => e.ExamEnrollmentId = 950)
            .Returns(Task.CompletedTask);

        _examNotificationServiceMock
            .Setup(n => n.NotifyEnrollmentAsync(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()))
            .ThrowsAsync(new InvalidOperationException("SMTP exploded"));

        var ids = await _service.EnrollAsync(1, new List<long> { 5 });

        ids.Should().ContainSingle().Which.Should().Be(950);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ReassignSeatAsync_RoomAtCapacity_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = new ExamEnrollment
        {
            ExamEnrollmentId = 1,
            ExamId = 1,
            ExamRoomId = null,
            Exam = ExamFor(1, 100, 10),
        };
        var room = new ExamRoom { ExamRoomId = 20, ExamVenueId = 10, RoomName = "Room A", Capacity = 2 };

        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);
        _examRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(room);
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsBySeatNumberAsync(1, "A-001", 1)).ReturnsAsync(false);
        _examEnrollmentRepositoryMock.Setup(r => r.GetCountByRoomIdAsync(20)).ReturnsAsync(2);

        var act = () => _service.ReassignSeatAsync(1, 20, "A-001");

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task ReassignSeatAsync_SeatNumberAlreadyTakenByAnotherEnrollment_ShouldThrowDuplicateException()
    {
        var enrollment = new ExamEnrollment
        {
            ExamEnrollmentId = 1,
            ExamId = 1,
            ExamRoomId = null,
            Exam = ExamFor(1, 100, 10),
        };
        var room = new ExamRoom { ExamRoomId = 20, ExamVenueId = 10, RoomName = "Room A", Capacity = 30 };

        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);
        _examRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(room);
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsBySeatNumberAsync(1, "A-001", 1)).ReturnsAsync(true);

        var act = () => _service.ReassignSeatAsync(1, 20, "A-001");

        await act.Should().ThrowAsync<DuplicateException>();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task ReassignSeatAsync_WithValidRoomAndSeat_ShouldUpdateEnrollment()
    {
        var enrollment = new ExamEnrollment
        {
            ExamEnrollmentId = 1,
            ExamId = 1,
            ExamRoomId = null,
            Exam = ExamFor(1, 100, 10),
        };
        var room = new ExamRoom { ExamRoomId = 20, ExamVenueId = 10, RoomName = "Room A", Capacity = 30 };

        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);
        _examRoomRepositoryMock.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(room);
        _examEnrollmentRepositoryMock.Setup(r => r.ExistsBySeatNumberAsync(1, "A-001", 1)).ReturnsAsync(false);
        _examEnrollmentRepositoryMock.Setup(r => r.GetCountByRoomIdAsync(20)).ReturnsAsync(0);

        await _service.ReassignSeatAsync(1, 20, "A-001");

        enrollment.ExamRoomId.Should().Be(20);
        enrollment.SeatNumber.Should().Be("A-001");
        _examEnrollmentRepositoryMock.Verify(r => r.Update(enrollment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UploadScoreAsync_ScoreAboveTotalMarks_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = new ExamEnrollment { ExamEnrollmentId = 1, ExamId = 1, Exam = ExamFor(1, 100, totalMarks: 100, passMarks: 40) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.UploadScoreAsync(1, 150);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task UploadScoreAsync_NegativeScore_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollment = new ExamEnrollment { ExamEnrollmentId = 1, ExamId = 1, Exam = ExamFor(1, 100, totalMarks: 100, passMarks: 40) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);

        var act = () => _service.UploadScoreAsync(1, -5);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task UploadScoreAsync_ValidScoreAboveOrEqualPassMarks_ShouldSetPassedAndManualUploadSource()
    {
        var enrollment = new ExamEnrollment { ExamEnrollmentId = 1, ExamId = 1, Exam = ExamFor(1, 100, totalMarks: 100, passMarks: 40) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);
        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("hr.abir");

        await _service.UploadScoreAsync(1, 45);

        enrollment.Score.Should().Be(45);
        enrollment.IsPassed.Should().BeTrue();
        enrollment.ScoreSource.Should().Be(ScoreSourceEnum.ManualUpload);
        enrollment.ScoredByUserName.Should().Be("hr.abir");
        enrollment.ScoredAt.Should().NotBeNull();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(enrollment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UploadScoreAsync_ScoreBelowPassMarks_ShouldSetIsPassedFalse()
    {
        var enrollment = new ExamEnrollment { ExamEnrollmentId = 1, ExamId = 1, Exam = ExamFor(1, 100, totalMarks: 100, passMarks: 40) };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(enrollment);

        await _service.UploadScoreAsync(1, 30);

        enrollment.IsPassed.Should().BeFalse();
    }

    // US-060 AC5: bulk-move all passing candidates on the exam Results page to a chosen pipeline
    // stage - re-validated server-side even though the frontend only lets HR select passing rows.

    [Fact]
    public async Task BulkMoveToStageAsync_AllPassing_ShouldDelegateToStageProgressServiceWithTheirJobApplicationIds()
    {
        var enrollments = new List<ExamEnrollment>
        {
            new() { ExamEnrollmentId = 1, ExamId = 1, JobApplicationId = 101, IsPassed = true },
            new() { ExamEnrollmentId = 2, ExamId = 1, JobApplicationId = 102, IsPassed = true },
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(enrollments);

        await _service.BulkMoveToStageAsync(1, new List<long> { 1, 2 }, 500);

        _jobApplicationStageProgressServiceMock.Verify(
            s => s.BulkAdvanceToStageAsync(
                It.Is<List<long>>(ids => ids.Count == 2 && ids.Contains(101) && ids.Contains(102)),
                500),
            Times.Once);
    }

    [Fact]
    public async Task BulkMoveToStageAsync_IncludesNonPassingEnrollment_ShouldThrowInvalidStatusTransitionException()
    {
        var enrollments = new List<ExamEnrollment>
        {
            new() { ExamEnrollmentId = 1, ExamId = 1, JobApplicationId = 101, IsPassed = true },
            new() { ExamEnrollmentId = 2, ExamId = 1, JobApplicationId = 102, IsPassed = false },
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(enrollments);

        var act = () => _service.BulkMoveToStageAsync(1, new List<long> { 1, 2 }, 500);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _jobApplicationStageProgressServiceMock.Verify(
            s => s.BulkAdvanceToStageAsync(It.IsAny<List<long>>(), It.IsAny<long>()), Times.Never);
    }

    [Fact]
    public async Task BulkMoveToStageAsync_UnknownEnrollmentIdForThisExam_ShouldThrowNotFoundException()
    {
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(new List<ExamEnrollment>());

        var act = () => _service.BulkMoveToStageAsync(1, new List<long> { 999 }, 500);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
