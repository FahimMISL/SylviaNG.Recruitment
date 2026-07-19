using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SylviaNG.Recruitment.Application.Common.Exceptions;
using SylviaNG.Recruitment.Application.Interfaces.Services;
using SylviaNG.Recruitment.Application.Services;
using SylviaNG.Recruitment.Domain.Entities;

namespace SylviaNG.Recruitment.Tests.Services;

public class AiShortlistScoringServiceTests
{
    private readonly Mock<IGroqClient> _groqClientMock;
    private readonly AiShortlistScoringService _service;

    public AiShortlistScoringServiceTests()
    {
        _groqClientMock = new Mock<IGroqClient>();
        _service = new AiShortlistScoringService(_groqClientMock.Object, Mock.Of<ILogger<AiShortlistScoringService>>());
    }

    private static JobPosting Posting() => new() { Title = "Software Engineer" };
    private static CandidateFactService.CandidateFacts Facts() =>
        new(30, 5, new HashSet<string>(), new HashSet<Domain.Enums.EducationLevelEnum>(), "Dhaka", new HashSet<string>());

    [Fact]
    public async Task ScoreAsync_ValidJsonResponse_ShouldReturnScoreAndExplanation()
    {
        _groqClientMock
            .Setup(c => c.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"score": 85, "explanation": "Strong match."}""");

        var result = await _service.ScoreAsync(Posting(), Facts());

        result.Failed.Should().BeFalse();
        result.Score.Should().Be(85);
        result.Explanation.Should().Be("Strong match.");
    }

    [Fact]
    public async Task ScoreAsync_ScoreOutOfRange_ShouldClamp()
    {
        _groqClientMock
            .Setup(c => c.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"score": 150, "explanation": "Overshoot."}""");

        var result = await _service.ScoreAsync(Posting(), Facts());

        result.Failed.Should().BeFalse();
        result.Score.Should().Be(100);
    }

    [Fact]
    public async Task ScoreAsync_MalformedJson_ShouldReturnFailedNotThrow()
    {
        _groqClientMock
            .Setup(c => c.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("not json");

        var result = await _service.ScoreAsync(Posting(), Facts());

        result.Failed.Should().BeTrue();
        result.Score.Should().BeNull();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ScoreAsync_MissingScoreProperty_ShouldReturnFailedNotThrow()
    {
        _groqClientMock
            .Setup(c => c.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("""{"explanation": "No score here."}""");

        var result = await _service.ScoreAsync(Posting(), Facts());

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public async Task ScoreAsync_GroqUnavailable_ShouldReturnFailedNotThrow()
    {
        _groqClientMock
            .Setup(c => c.GetJsonCompletionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new GroqUnavailableException("Groq server is unreachable."));

        var act = () => _service.ScoreAsync(Posting(), Facts());

        var result = await act.Should().NotThrowAsync();
        result.Subject.Failed.Should().BeTrue();
        result.Subject.ErrorMessage.Should().Be("Groq server is unreachable.");
    }
}
