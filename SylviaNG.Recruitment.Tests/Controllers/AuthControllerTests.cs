using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Login;
using SylviaNG.Recruitment.Application.Features.Auth.Commands.Register;
using SylviaNG.Recruitment.Application.Features.Auth.Models;
using SylviaNG.Recruitment.Controllers;

namespace SylviaNG.Recruitment.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new AuthController(_mediatorMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithLoginResponse()
    {
        // Arrange
        var request = new LoginRequest { Username = "admin", Password = "admin123" };
        var expected = new LoginResponse
        {
            Token = "token",
            Username = "admin",
            DisplayName = "Administrator",
            Role = "Admin",
            ExpiresAtUtc = DateTime.UtcNow.AddHours(1)
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnOkWithRegisterResponse()
    {
        // Arrange
        var request = new RegisterRequest { FullName = "Kamal Hossain", Email = "kamal@example.com", Password = "secret-pass-1" };
        var expected = new RegisterResponse { Email = "kamal@example.com", RequiresEmailVerification = true };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), default))
            .ReturnsAsync(expected);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
