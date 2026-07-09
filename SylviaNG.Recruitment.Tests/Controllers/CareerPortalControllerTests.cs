using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Recruitment.Application.Features.JobPostings.Commands.JobApplicationSubmit;
using SylviaNG.Recruitment.Application.Features.JobPostings.Models;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetAllPublicPaged;
using SylviaNG.Recruitment.Application.Features.JobPostings.Queries.JobPostingGetPublicById;
using SylviaNG.Recruitment.Controllers;
using SylviaNG.Recruitment.Domain.Enums;
using SylviaNG.Recruitment.SharedKernel.Pagination;

namespace SylviaNG.Recruitment.Tests.Controllers;

public class CareerPortalControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CareerPortalController _controller;

    public CareerPortalControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CareerPortalController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithPagedJobPostings()
    {
        // Arrange
        var expected = new PagedResult<JobPostingResponse>
        {
            Data = new List<JobPostingResponse>
            {
                new() { JobPostingId = 1, Title = "Software Engineer", CircularType = CircularTypeEnum.Both }
            },
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<JobPostingGetAllPublicPagedQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetAll(new PagedRequest(), null, null, null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithJobPosting()
    {
        // Arrange
        var expected = new JobPostingResponse { JobPostingId = 1, Title = "Software Engineer" };

        _mediatorMock.Setup(m => m.Send(It.IsAny<JobPostingGetPublicByIdQuery>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Apply_ShouldSendCommandWithExternalSourceAndRouteJobPostingId_AndReturnOk()
    {
        // Arrange
        var expected = new JobApplicationResponse { JobApplicationId = 1, JobPostingId = 5, Source = ApplicationSourceEnum.External };
        JobApplicationSubmitCommand? capturedCommand = null;

        _mediatorMock.Setup(m => m.Send(It.IsAny<JobApplicationSubmitCommand>(), default))
            .Callback<IRequest<JobApplicationResponse>, CancellationToken>((cmd, _) => capturedCommand = (JobApplicationSubmitCommand)cmd)
            .ReturnsAsync(expected);

        var request = new JobApplicationSubmitRequest
        {
            CandidateName = "Jane Doe",
            CandidateEmail = "jane@example.com"
        };

        // Act
        var result = await _controller.Apply(5, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
        capturedCommand.Should().NotBeNull();
        capturedCommand!.Source.Should().Be(ApplicationSourceEnum.External);
        capturedCommand.Request.JobPostingId.Should().Be(5);
    }
}
