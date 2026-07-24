using System.IO.Compression;
using System.Text;
using ClosedXML.Excel;
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

public class ExamSeatPlanServiceTests
{
    private readonly Mock<IExamRepository> _examRepositoryMock;
    private readonly Mock<IExamEnrollmentRepository> _examEnrollmentRepositoryMock;
    private readonly Mock<IExamRoomRepository> _examRoomRepositoryMock;
    private readonly Mock<ISeatPlanPdfGeneratorService> _seatPlanPdfGeneratorServiceMock;
    private readonly Mock<IAdmitCardPdfGeneratorService> _admitCardPdfGeneratorServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExamSeatPlanService _service;

    public ExamSeatPlanServiceTests()
    {
        _examRepositoryMock = new Mock<IExamRepository>();
        _examEnrollmentRepositoryMock = new Mock<IExamEnrollmentRepository>();
        _examRoomRepositoryMock = new Mock<IExamRoomRepository>();
        _seatPlanPdfGeneratorServiceMock = new Mock<ISeatPlanPdfGeneratorService>();
        _admitCardPdfGeneratorServiceMock = new Mock<IAdmitCardPdfGeneratorService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExamSeatPlanService(
            _examRepositoryMock.Object,
            _examEnrollmentRepositoryMock.Object,
            _examRoomRepositoryMock.Object,
            _seatPlanPdfGeneratorServiceMock.Object,
            _admitCardPdfGeneratorServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static Exam InPersonExam(long examId = 1, long examVenueId = 10) => new()
    {
        ExamId = examId,
        JobPostingId = 100,
        ExamType = ExamTypeEnum.InPerson,
        ExamVenueId = examVenueId,
    };

    private static ExamEnrollment EnrollmentFor(long id, long examId) => new()
    {
        ExamEnrollmentId = id,
        ExamId = examId,
        JobApplicationId = id,
        JobApplication = new JobApplication { JobApplicationId = id, CandidateName = "Candidate " + id },
    };

    [Fact]
    public async Task GenerateAsync_ExamNotInPerson_ShouldThrowInvalidStatusTransitionException()
    {
        var exam = InPersonExam();
        exam.ExamType = ExamTypeEnum.Online;
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);

        var act = () => _service.GenerateAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task GenerateAsync_InPersonExamWithoutVenue_ShouldThrowInvalidStatusTransitionException()
    {
        var exam = InPersonExam();
        exam.ExamVenueId = null;
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);

        var act = () => _service.GenerateAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
    }

    [Fact]
    public async Task GenerateAsync_TotalEnrolledExceedsTotalCapacity_ShouldThrowInvalidStatusTransitionException()
    {
        var exam = InPersonExam();
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);
        _examRoomRepositoryMock.Setup(r => r.GetActiveByVenueIdAsync(10)).ReturnsAsync(new List<ExamRoom>
        {
            new() { ExamRoomId = 1, ExamVenueId = 10, RoomName = "Room A", Capacity = 2, IsActive = true },
        });
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(new List<ExamEnrollment>
        {
            EnrollmentFor(1, 1), EnrollmentFor(2, 1), EnrollmentFor(3, 1),
        });

        var act = () => _service.GenerateAsync(1);

        await act.Should().ThrowAsync<InvalidStatusTransitionException>();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Never);
    }

    [Fact]
    public async Task GenerateAsync_DistributesCandidatesAcrossRoomsWithUniqueSequentialSeatNumbers()
    {
        var exam = InPersonExam();
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);

        var roomA = new ExamRoom { ExamRoomId = 1, ExamVenueId = 10, RoomName = "Room A", Capacity = 2, IsActive = true };
        var roomB = new ExamRoom { ExamRoomId = 2, ExamVenueId = 10, RoomName = "Room B", Capacity = 2, IsActive = true };
        _examRoomRepositoryMock.Setup(r => r.GetActiveByVenueIdAsync(10)).ReturnsAsync(new List<ExamRoom> { roomA, roomB });

        var enrollments = new List<ExamEnrollment>
        {
            EnrollmentFor(1, 1), EnrollmentFor(2, 1), EnrollmentFor(3, 1),
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(enrollments);

        await _service.GenerateAsync(1);

        // Room A (2 capacity) fills first with seats A-001, A-002; overflow goes to Room B seat B-001.
        enrollments[0].ExamRoomId.Should().Be(1);
        enrollments[0].SeatNumber.Should().Be("Room A-001");
        enrollments[1].ExamRoomId.Should().Be(1);
        enrollments[1].SeatNumber.Should().Be("Room A-002");
        enrollments[2].ExamRoomId.Should().Be(2);
        enrollments[2].SeatNumber.Should().Be("Room B-001");

        enrollments.Select(e => e.SeatNumber).Distinct().Should().HaveCount(3);
        exam.SeatPlanGeneratedAt.Should().NotBeNull();
        _examEnrollmentRepositoryMock.Verify(r => r.Update(It.IsAny<ExamEnrollment>()), Times.Exactly(3));
        _examRepositoryMock.Verify(r => r.Update(exam), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateAsync_CalledTwice_ShouldResetAndRebuildAssignmentsFromScratch()
    {
        var exam = InPersonExam();
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);

        var roomA = new ExamRoom { ExamRoomId = 1, ExamVenueId = 10, RoomName = "Room A", Capacity = 2, IsActive = true };
        var roomB = new ExamRoom { ExamRoomId = 2, ExamVenueId = 10, RoomName = "Room B", Capacity = 2, IsActive = true };
        _examRoomRepositoryMock.Setup(r => r.GetActiveByVenueIdAsync(10)).ReturnsAsync(new List<ExamRoom> { roomA, roomB });

        var enrollments = new List<ExamEnrollment>
        {
            EnrollmentFor(1, 1), EnrollmentFor(2, 1), EnrollmentFor(3, 1),
        };
        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(enrollments);

        await _service.GenerateAsync(1);

        // Simulate a manual reassignment after the first generation.
        enrollments[0].ExamRoomId = 2;
        enrollments[0].SeatNumber = "Manually-Moved";

        await _service.GenerateAsync(1);

        // Regeneration walks the same rooms/queue order again and must overwrite the manual change.
        enrollments[0].ExamRoomId.Should().Be(1);
        enrollments[0].SeatNumber.Should().Be("Room A-001");
        enrollments[1].ExamRoomId.Should().Be(1);
        enrollments[1].SeatNumber.Should().Be("Room A-002");
        enrollments[2].ExamRoomId.Should().Be(2);
        enrollments[2].SeatNumber.Should().Be("Room B-001");
    }

    [Fact]
    public async Task GenerateAdmitCardZipAsync_ShouldReturnOnePdfEntryPerEnrollment()
    {
        var exam = InPersonExam();
        var enrollments = new List<ExamEnrollment>
        {
            EnrollmentFor(1, 1), EnrollmentFor(2, 1),
        };
        foreach (var e in enrollments) e.Exam = exam;

        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdWithDetailsAsync(1)).ReturnsAsync(enrollments);
        _admitCardPdfGeneratorServiceMock
            .Setup(g => g.Generate(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()))
            .Returns(Encoding.UTF8.GetBytes("pdf-bytes"));

        var (content, fileName) = await _service.GenerateAdmitCardZipAsync(1);

        fileName.Should().EndWith(".zip");
        using var zipStream = new MemoryStream(content);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        archive.Entries.Should().HaveCount(2);
        archive.Entries.Select(e => e.Name).Should().OnlyHaveUniqueItems();
        _admitCardPdfGeneratorServiceMock.Verify(
            g => g.Generate(It.IsAny<ExamEnrollment>(), It.IsAny<Exam>(), It.IsAny<JobApplication>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task GenerateResultsExcelAsync_ShouldSortByScoreDescendingAndMarkPassFail()
    {
        var exam = InPersonExam();
        exam.TotalMarks = 100;
        exam.PassMarks = 40;
        _examRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(exam);

        var low = EnrollmentFor(1, 1);
        low.Score = 30;
        low.IsPassed = false;
        var high = EnrollmentFor(2, 1);
        high.Score = 90;
        high.IsPassed = true;
        var unscored = EnrollmentFor(3, 1);

        _examEnrollmentRepositoryMock.Setup(r => r.GetByExamIdAsync(1)).ReturnsAsync(new List<ExamEnrollment> { low, high, unscored });

        var (content, fileName) = await _service.GenerateResultsExcelAsync(1);

        fileName.Should().EndWith(".xlsx");
        using var stream = new MemoryStream(content);
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet("Results");

        sheet.Cell(2, 1).GetString().Should().Be(high.JobApplication.CandidateName);
        sheet.Cell(2, 5).GetString().Should().Be("Pass");
        sheet.Cell(3, 1).GetString().Should().Be(low.JobApplication.CandidateName);
        sheet.Cell(3, 5).GetString().Should().Be("Fail");
        sheet.Cell(4, 5).GetString().Should().Be("Not Scored");
    }
}
