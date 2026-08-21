using FluentAssertions;
using SylviaNG.Recruitment.Application.Common.BooleanQuery;

namespace SylviaNG.Recruitment.Tests.Services;

public class BooleanQueryParserTests
{
    [Theory]
    [InlineData("react", "I know React and Node", true)]
    [InlineData("angular", "I know React and Node", false)]
    public void Parse_SingleTerm_MatchesCaseInsensitiveSubstring(string query, string haystack, bool expected)
    {
        var node = BooleanQueryParser.Parse(query);
        node.Evaluate(haystack).Should().Be(expected);
    }

    [Theory]
    [InlineData("react AND node", "Experienced in React and Node.js", true)]
    [InlineData("react AND angular", "Experienced in React and Node.js", false)]
    [InlineData("react node", "Experienced in React and Node.js", true)] // implicit AND
    [InlineData("react angular", "Experienced in React and Node.js", false)] // implicit AND
    public void Parse_And_RequiresBothTerms(string query, string haystack, bool expected)
    {
        var node = BooleanQueryParser.Parse(query);
        node.Evaluate(haystack).Should().Be(expected);
    }

    [Theory]
    [InlineData("react OR angular", "Experienced in React only", true)]
    [InlineData("vue OR angular", "Experienced in React only", false)]
    public void Parse_Or_RequiresEitherTerm(string query, string haystack, bool expected)
    {
        var node = BooleanQueryParser.Parse(query);
        node.Evaluate(haystack).Should().Be(expected);
    }

    [Theory]
    [InlineData("react NOT intern", "Senior React developer", true)]
    [InlineData("react NOT intern", "React intern", false)]
    public void Parse_Not_ExcludesMatches(string query, string haystack, bool expected)
    {
        var node = BooleanQueryParser.Parse(query);
        node.Evaluate(haystack).Should().Be(expected);
    }

    [Theory]
    [InlineData("react AND (node OR angular)", "React and Angular developer", true)]
    [InlineData("react AND (node OR angular)", "React and Vue developer", false)]
    public void Parse_Parentheses_OverridePrecedence(string query, string haystack, bool expected)
    {
        var node = BooleanQueryParser.Parse(query);
        node.Evaluate(haystack).Should().Be(expected);
    }

    [Fact]
    public void Parse_QuotedPhrase_MatchesAsSingleTerm()
    {
        var node = BooleanQueryParser.Parse("\"project manager\"");

        node.Evaluate("Senior Project Manager with 5 years").Should().BeTrue();
        node.Evaluate("Project lead and Manager of teams").Should().BeFalse();
    }

    [Fact]
    public void Parse_EmptyQuery_Throws()
    {
        var act = () => BooleanQueryParser.Parse("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Parse_UnbalancedParens_Throws()
    {
        var act = () => BooleanQueryParser.Parse("(react AND node");
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void CountMatchedTerms_CountsDistinctPositiveTermsPresent_ExcludingNotTerms()
    {
        var node = BooleanQueryParser.Parse("react AND node NOT angular");

        // "react" and "node" both present -> 2; "angular" is a NOT-term, never counted.
        BooleanQueryParser.CountMatchedTerms(node, "react and node developer").Should().Be(2);
    }

    [Fact]
    public void CountMatchedTerms_OnlyCountsTermsActuallyPresent()
    {
        var node = BooleanQueryParser.Parse("react OR angular OR vue");

        BooleanQueryParser.CountMatchedTerms(node, "react developer").Should().Be(1);
    }
}
