using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentDelete;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Commands.JobPostingAttachmentUpload;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Models;
using SylviaNG.Recruitment.Application.Features.JobPostingAttachments.Queries.JobPostingAttachmentGetAllByJobPosting;
using SylviaNG.Recruitment.Controllers;

namespace SylviaNG.Recruitment.Tests.Controllers;

public class JobPostingAttachmentControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly JobPostingAttachmentController _controller;

    public JobPostingAttachmentControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new JobPostingAttachmentController(_mediatorMock.Object);
    }

    private static IFormFile CreateFormFile(string fileName = "resume.pdf")
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes("dummy content");
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "file", fileName);
    }

    [Fact]
    public async Task Upload_WithValidFile_ShouldReturnOkWithAttachmentResponse()
    {
        // Arrange
        var expected = new JobPostingAttachmentResponse
        {
            JobPostingAttachmentId = 1,
            JobPostingId = 1,
            FileName = "resume.pdf",
            DownloadUrl = "/uploads/job-postings/1/abc123.pdf"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<JobPostingAttachmentUploadCommand>(), default))
            .ReturnsAsync(expected);

        var file = CreateFormFile();

        // Act
        var result = await _controller.Upload(1, file);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithAttachmentList()
    {
        // Arrange
        var expected = new List<JobPostingAttachmentResponse>
        {
            new() { JobPostingAttachmentId = 1, JobPostingId = 1, FileName = "a.pdf" },
            new() { JobPostingAttachmentId = 2, JobPostingId = 1, FileName = "b.pdf" }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<JobPostingAttachmentGetAllByJobPostingQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAll(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Delete_WithValidIds_ShouldReturnOk()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<JobPostingAttachmentDeleteCommand>(), default))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.Delete(1, 10);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}
