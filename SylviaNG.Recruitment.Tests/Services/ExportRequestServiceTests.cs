using FluentAssertions;
using FluentValidation;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.ExportRequests.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Tests.Services;

public class ExportRequestServiceTests
{
    private readonly Mock<IExportRequestRepository> _exportRequestRepositoryMock;
    private readonly Mock<IJobApplicationService> _jobApplicationServiceMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ExportRequestService _service;

    public ExportRequestServiceTests()
    {
        _exportRequestRepositoryMock = new Mock<IExportRequestRepository>();
        _jobApplicationServiceMock = new Mock<IJobApplicationService>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ExportRequestService(
            _exportRequestRepositoryMock.Object,
            _jobApplicationServiceMock.Object,
            _currentUserServiceMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task RequestCandidateListExportAsync_ValidFilter_ShouldQueuePendingRowWithMatchedIds()
    {
        var filter = new JobApplicationAttributeFilterRequest { JobPostingId = 7 };
        _jobApplicationServiceMock.Setup(s => s.GetDashboardMatchingIdsAsync(filter))
            .ReturnsAsync(new List<long> { 1, 2, 3 });
        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("abir");
        _currentUserServiceMock.Setup(s => s.GetCurrentUserEmail()).Returns("abir@example.com");

        ExportRequest? captured = null;
        _exportRequestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExportRequest>()))
            .Callback<ExportRequest>(e => captured = e)
            .Returns(Task.CompletedTask);

        await _service.RequestCandidateListExportAsync(filter, ExportFormatEnum.Xlsx);

        captured.Should().NotBeNull();
        captured!.Status.Should().Be(ExportRequestStatusEnum.Pending);
        captured.RowCount.Should().Be(3);
        captured.RequestedByUserName.Should().Be("abir");
        captured.RequestedByEmail.Should().Be("abir@example.com");
        captured.JobApplicationIdsJson.Should().Be("[1,2,3]");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RequestCandidateListExportAsync_InvalidFilter_ShouldPropagateValidationException()
    {
        var filter = new JobApplicationAttributeFilterRequest { Skills = new List<string> { "C#" } };
        _jobApplicationServiceMock.Setup(s => s.GetDashboardMatchingIdsAsync(filter))
            .ThrowsAsync(new ValidationException("JobPostingId is required."));

        var act = () => _service.RequestCandidateListExportAsync(filter, ExportFormatEnum.Xlsx);

        await act.Should().ThrowAsync<ValidationException>();
        _exportRequestRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExportRequest>()), Times.Never);
    }

    [Fact]
    public async Task RequestBulkCvZipExportAsync_ValidIds_ShouldQueuePendingBulkCvZipRow()
    {
        _currentUserServiceMock.Setup(s => s.GetCurrentUserName()).Returns("abir");
        _currentUserServiceMock.Setup(s => s.GetCurrentUserEmail()).Returns("abir@example.com");

        ExportRequest? captured = null;
        _exportRequestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ExportRequest>()))
            .Callback<ExportRequest>(e => captured = e)
            .Returns(Task.CompletedTask);

        await _service.RequestBulkCvZipExportAsync(new List<long> { 5, 6, 6 });

        captured.Should().NotBeNull();
        captured!.ExportType.Should().Be(ExportTypeEnum.BulkCvZip);
        captured.Format.Should().Be(ExportFormatEnum.Zip);
        captured.Status.Should().Be(ExportRequestStatusEnum.Pending);
        captured.JobApplicationIdsJson.Should().Be("[5,6]");
        captured.RowCount.Should().Be(2);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RequestBulkCvZipExportAsync_EmptyIds_ShouldThrowValidationException()
    {
        var act = () => _service.RequestBulkCvZipExportAsync(new List<long>());

        await act.Should().ThrowAsync<ValidationException>();
        _exportRequestRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExportRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetForDownloadAsync_UnknownId_ShouldThrowNotFoundException()
    {
        _exportRequestRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ExportRequest?)null);

        var act = () => _service.GetForDownloadAsync(99);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetForDownloadAsync_NotYetCompleted_ShouldThrowValidationException()
    {
        _exportRequestRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ExportRequest { ExportRequestId = 1, Status = ExportRequestStatusEnum.Processing });

        var act = () => _service.GetForDownloadAsync(1);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task GetForDownloadAsync_Completed_ShouldReturnFile()
    {
        _exportRequestRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new ExportRequest
            {
                ExportRequestId = 1,
                Status = ExportRequestStatusEnum.Completed,
                ContentObjectKey = "exports/candidate-list-20260727.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = "Candidate-List-Export-20260727.xlsx"
            });
        _fileStorageServiceMock.Setup(f => f.OpenReadAsync("exports/candidate-list-20260727.xlsx"))
            .ReturnsAsync(() => new MemoryStream(new byte[] { 1, 2, 3 }));

        var result = await _service.GetForDownloadAsync(1);

        using var buffer = new MemoryStream();
        await result.Content.CopyToAsync(buffer);
        buffer.ToArray().Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        result.FileName.Should().Be("Candidate-List-Export-20260727.xlsx");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldMapEntitiesToResponses()
    {
        _exportRequestRepositoryMock.Setup(r => r.GetPagedAsync(1, 20, null))
            .ReturnsAsync(new PagedResult<ExportRequest>
            {
                Data = new List<ExportRequest> { new() { ExportRequestId = 1, Status = ExportRequestStatusEnum.Completed } },
                PageNumber = 1,
                PageSize = 20,
                TotalCount = 1
            });

        var result = await _service.GetPagedAsync(new ExportRequestFilterRequest { Page = 1, PageSize = 20 });

        result.Data.Should().ContainSingle(r => r.ExportRequestId == 1 && r.Status == ExportRequestStatusEnum.Completed);
    }
}
