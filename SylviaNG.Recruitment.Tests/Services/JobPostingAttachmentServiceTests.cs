using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobPostingAttachmentServiceTests
{
    private readonly Mock<IJobPostingAttachmentRepository> _attachmentRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobPostingAttachmentService _service;

    public JobPostingAttachmentServiceTests()
    {
        _attachmentRepositoryMock = new Mock<IJobPostingAttachmentRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new JobPostingAttachmentService(
            _attachmentRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static IFormFile CreateFormFile(string fileName = "resume.pdf", string content = "dummy content")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
    }

    [Fact]
    public async Task UploadAsync_WithExistingJobPosting_ShouldSaveFileAndReturnResponse()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Test" };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);

        _fileStorageServiceMock
            .Setup(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), "1"))
            .ReturnsAsync(("abc123.pdf", "uploads/job-postings/1/abc123.pdf"));

        JobPostingAttachment? savedEntity = null;
        _attachmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobPostingAttachment>()))
            .Callback<JobPostingAttachment>(a =>
            {
                a.JobPostingAttachmentId = 10;
                savedEntity = a;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var file = CreateFormFile();

        // Act
        var result = await _service.UploadAsync(1, file);

        // Assert
        result.Should().NotBeNull();
        result.JobPostingId.Should().Be(1);
        result.FileName.Should().Be("resume.pdf");
        result.DownloadUrl.Should().Be("/uploads/job-postings/1/abc123.pdf");
        savedEntity.Should().NotBeNull();
        savedEntity!.StoredFileName.Should().Be("abc123.pdf");
        _attachmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobPostingAttachment>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UploadAsync_WithNonExistentJobPosting_ShouldThrowNotFoundException()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobPosting?)null);
        var file = CreateFormFile();

        // Act
        var act = () => _service.UploadAsync(999, file);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _fileStorageServiceMock.Verify(f => f.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingAttachment_ShouldDeleteFileAndRecord()
    {
        // Arrange
        var entity = new JobPostingAttachment
        {
            JobPostingAttachmentId = 10,
            JobPostingId = 1,
            FilePath = "uploads/job-postings/1/abc123.pdf"
        };
        _attachmentRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(entity);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(1, 10);

        // Assert
        _attachmentRepositoryMock.Verify(r => r.Delete(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _fileStorageServiceMock.Verify(f => f.DeleteAsync(entity.FilePath), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentAttachment_ShouldThrowNotFoundException()
    {
        // Arrange
        _attachmentRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobPostingAttachment?)null);

        // Act
        var act = () => _service.DeleteAsync(1, 999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithAttachmentBelongingToDifferentJobPosting_ShouldThrowNotFoundException()
    {
        // Arrange
        var entity = new JobPostingAttachment { JobPostingAttachmentId = 10, JobPostingId = 2 };
        _attachmentRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(entity);

        // Act
        var act = () => _service.DeleteAsync(1, 10);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetAllByJobPostingIdAsync_WithExistingJobPosting_ShouldReturnMappedResponses()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Test" };
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(jobPosting);

        var attachments = new List<JobPostingAttachment>
        {
            new() { JobPostingAttachmentId = 1, JobPostingId = 1, FileName = "a.pdf", FilePath = "uploads/job-postings/1/a.pdf" },
            new() { JobPostingAttachmentId = 2, JobPostingId = 1, FileName = "b.pdf", FilePath = "uploads/job-postings/1/b.pdf" }
        };
        _attachmentRepositoryMock.Setup(r => r.GetAllByJobPostingIdAsync(1)).ReturnsAsync(attachments);

        // Act
        var result = await _service.GetAllByJobPostingIdAsync(1);

        // Assert
        result.Should().HaveCount(2);
        result.Select(r => r.FileName).Should().Contain(new[] { "a.pdf", "b.pdf" });
    }

    [Fact]
    public async Task GetAllByJobPostingIdAsync_WithNonExistentJobPosting_ShouldThrowNotFoundException()
    {
        // Arrange
        _jobPostingRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((JobPosting?)null);

        // Act
        var act = () => _service.GetAllByJobPostingIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
