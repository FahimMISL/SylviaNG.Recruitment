using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Interfaces.Repositories;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Generic;

namespace SylviaNG.Recruitment.Tests.Services;

public class JobApplicationServiceTests
{
    private readonly Mock<IJobApplicationRepository> _jobApplicationRepositoryMock;
    private readonly Mock<IJobPostingRepository> _jobPostingRepositoryMock;
    private readonly Mock<IApplicationCvStorageService> _cvStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly JobApplicationService _service;

    public JobApplicationServiceTests()
    {
        _jobApplicationRepositoryMock = new Mock<IJobApplicationRepository>();
        _jobPostingRepositoryMock = new Mock<IJobPostingRepository>();
        _cvStorageServiceMock = new Mock<IApplicationCvStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new JobApplicationService(
            _jobApplicationRepositoryMock.Object,
            _jobPostingRepositoryMock.Object,
            _cvStorageServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private static IFormFile CreateFormFile(string fileName = "resume.pdf", string content = "dummy content")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "resume", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/pdf"
        };
    }

    private static JobApplicationSubmitRequest CreateRequest(long jobPostingId = 1, string email = "jane@example.com", IFormFile? resume = null)
    {
        return new JobApplicationSubmitRequest
        {
            JobPostingId = jobPostingId,
            CandidateName = "Jane Doe",
            CandidateEmail = email,
            CandidatePhone = "+880123456789",
            CoverLetter = "I am interested in this role.",
            Resume = resume
        };
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WithValidRequestAndOpenMatchingPosting_ShouldSaveApplicationAndReturnResponse(ApplicationSourceEnum source)
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        var resume = CreateFormFile();
        _cvStorageServiceMock
            .Setup(s => s.SaveAsync(It.IsAny<Stream>(), "resume.pdf", "1"))
            .ReturnsAsync(("abc123.pdf", "uploads/applications/1/abc123.pdf"));

        JobApplication? savedEntity = null;
        _jobApplicationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<JobApplication>()))
            .Callback<JobApplication>(a =>
            {
                a.JobApplicationId = 10;
                savedEntity = a;
            });

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: resume);

        // Act
        var result = await _service.SubmitAsync(request, source);

        // Assert
        result.Should().NotBeNull();
        result.JobApplicationId.Should().Be(10);
        result.Source.Should().Be(source);
        result.ResumeUrl.Should().Be("uploads/applications/1/abc123.pdf");
        savedEntity.Should().NotBeNull();
        savedEntity!.Source.Should().Be(source);
        savedEntity.ApplicationStatus.Should().Be(ApplicationStatusEnum.Applied);
        savedEntity.AppliedDate.Should().NotBeNull();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_WithoutResume_ShouldNotCallCvStorageAndShouldSaveWithNullResumeUrl()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync((JobApplication?)null);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = CreateRequest(resume: null);

        // Act
        var result = await _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        result.ResumeUrl.Should().BeNull();
        _cvStorageServiceMock.Verify(s => s.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SubmitAsync_WithDuplicateEmailForSameJobPosting_ShouldThrowDuplicateException()
    {
        // Arrange
        var jobPosting = new JobPosting { JobPostingId = 1, Title = "Software Engineer", Status = JobStatusEnum.Open };
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync(jobPosting);

        _jobApplicationRepositoryMock
            .Setup(r => r.GetByEmailAndJobPostingIdAsync("jane@example.com", 1))
            .ReturnsAsync(new JobApplication { JobApplicationId = 5, JobPostingId = 1, CandidateEmail = "jane@example.com" });

        var request = CreateRequest();

        // Act
        var act = () => _service.SubmitAsync(request, ApplicationSourceEnum.External);

        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
    }

    [Theory]
    [InlineData(ApplicationSourceEnum.External)]
    [InlineData(ApplicationSourceEnum.Internal)]
    public async Task SubmitAsync_WithNoMatchingOpenPostingForAudience_ShouldThrowNotFoundException(ApplicationSourceEnum source)
    {
        // Arrange: repository returns null when the posting is not Open or the CircularType
        // doesn't match the requested audience (e.g. an InternalOnly posting reached via the
        // public career portal, or vice-versa).
        _jobPostingRepositoryMock
            .Setup(r => r.GetOpenByIdAndCircularTypesAsync(1, It.IsAny<IReadOnlyCollection<CircularTypeEnum>>()))
            .ReturnsAsync((JobPosting?)null);

        var request = CreateRequest();

        // Act
        var act = () => _service.SubmitAsync(request, source);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _jobApplicationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<JobApplication>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
