using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SylviaNG.Recruitment.Application.Common.Email;
using SylviaNG.Recruitment.Application.Common.Settings;
using SylviaNG.Recruitment.Infrastructure.Services;

namespace SylviaNG.Recruitment.Tests.Services;

public class SmtpEmailServiceTests
{
    private static SmtpEmailService CreateService(SmtpSettings settings)
    {
        var options = Options.Create(settings);
        return new SmtpEmailService(options, new Mock<ILogger<SmtpEmailService>>().Object);
    }

    private static EmailMessage Message() => new()
    {
        To = "candidate@example.com",
        Subject = "Exam Notification",
        HtmlBody = "<p>You are enrolled.</p>",
    };

    [Fact]
    public async Task TrySendAsync_WhenNotEnabled_ShouldReturnFailureWithoutThrowing()
    {
        var settings = new SmtpSettings
        {
            IsEnabled = false,
            Host = "smtp.example.com",
        };
        var service = CreateService(settings);

        var result = await service.TrySendAsync(Message());

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task TrySendAsync_WhenHostIsBlank_ShouldReturnFailureWithoutThrowing()
    {
        var settings = new SmtpSettings
        {
            IsEnabled = true,
            Host = null,
        };
        var service = CreateService(settings);

        var result = await service.TrySendAsync(Message());

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task TrySendAsync_WhenNotConfigured_ShouldReturnQuicklyWithoutAttemptingNetworkConnection()
    {
        var settings = new SmtpSettings
        {
            IsEnabled = false,
            Host = "unreachable.invalid",
            Port = 587,
        };
        var service = CreateService(settings);

        var task = service.TrySendAsync(Message());
        var completedTask = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(2)));

        completedTask.Should().Be(task, "a disabled SMTP sender must short-circuit rather than attempt a network connection");
        (await task).Success.Should().BeFalse();
    }
}
