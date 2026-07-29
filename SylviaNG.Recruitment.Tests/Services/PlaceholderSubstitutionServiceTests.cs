using FluentAssertions;
using SylviaNG.Recruitment.Application.Common.Notifications;

namespace SylviaNG.Recruitment.Tests.Services;

public class PlaceholderSubstitutionServiceTests
{
    private readonly PlaceholderSubstitutionService _service = new();

    [Fact]
    public void Render_KnownPlaceholders_ShouldSubstituteValues()
    {
        var result = _service.Render(
            "Hi {{CandidateName}}, your exam is on {{ExamDate}}.",
            new Dictionary<string, string> { ["CandidateName"] = "Abir", ["ExamDate"] = "2026-08-01" });

        result.Should().Be("Hi Abir, your exam is on 2026-08-01.");
    }

    [Fact]
    public void Render_IsCaseInsensitive()
    {
        var result = _service.Render("Hi {{candidatename}}", new Dictionary<string, string> { ["CandidateName"] = "Abir" });

        result.Should().Be("Hi Abir");
    }

    [Fact]
    public void Render_UnmatchedPlaceholder_ShouldLeaveTokenVisible()
    {
        var result = _service.Render("Hi {{CandidateName}}, ref {{MissingToken}}", new Dictionary<string, string> { ["CandidateName"] = "Abir" });

        result.Should().Be("Hi Abir, ref {{MissingToken}}");
    }

    [Fact]
    public void ExtractPlaceholders_ShouldReturnDistinctTokensInFirstSeenOrder()
    {
        var result = _service.ExtractPlaceholders("{{CandidateName}} - {{ExamDate}} - {{CandidateName}}");

        result.Should().Equal("CandidateName", "ExamDate");
    }

    [Fact]
    public void ExtractPlaceholders_NoPlaceholders_ShouldReturnEmptyList()
    {
        var result = _service.ExtractPlaceholders("No tokens here.");

        result.Should().BeEmpty();
    }
}
