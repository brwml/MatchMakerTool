namespace Reporting.Test.Exporters;

using System.Globalization;

using MatchMaker.Reporting.Exporters;

using Xunit;

public class SafeMarkdownTests
{
    [Fact]
    public void Constructor_WithValue_ExposesRawValue()
    {
        var safeMarkdown = new SafeMarkdown("*bold*");

        Assert.Equal("*bold*", safeMarkdown.Value);
    }

    [Fact]
    public void Constructor_WithNull_ExposesEmptyString()
    {
        var safeMarkdown = new SafeMarkdown(null!);

        Assert.Equal(string.Empty, safeMarkdown.Value);
    }

    [Fact]
    public void ToString_ReturnsRawUnescapedValue()
    {
        var safeMarkdown = new SafeMarkdown("*bold* [link](url)");

        Assert.Equal("*bold* [link](url)", safeMarkdown.ToString());
    }
}

public class MarkdownEscapeRendererTests
{
    private readonly MarkdownEscapeRenderer renderer = new();

    [Fact]
    public void ToString_WithNull_ReturnsEmptyString()
    {
        var result = this.renderer.ToString(null!, null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void ToString_WithNonSafeMarkdownObject_ReturnsObjectToString()
    {
        var result = this.renderer.ToString(42, null!, CultureInfo.InvariantCulture);

        Assert.Equal("42", result);
    }

    [Fact]
    public void ToString_WithEmptySafeMarkdown_ReturnsEmptyString()
    {
        var result = this.renderer.ToString(new SafeMarkdown(string.Empty), null!, CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("|", "\\|")]
    [InlineData("[", "\\[")]
    [InlineData("]", "\\]")]
    [InlineData("(", "\\(")]
    [InlineData(")", "\\)")]
    [InlineData(@"\", @"\\")]
    [InlineData("*", "\\*")]
    [InlineData("_", "\\_")]
    [InlineData("#", "\\#")]
    [InlineData("+", "\\+")]
    [InlineData("-", "\\-")]
    [InlineData(".", "\\.")]
    [InlineData("!", "\\!")]
    [InlineData("`", "\\`")]
    [InlineData("{", "\\{")]
    [InlineData("}", "\\}")]
    public void ToString_WithSpecialCharacter_EscapesCharacter(string input, string expected)
    {
        var result = this.renderer.ToString(new SafeMarkdown(input), null!, CultureInfo.InvariantCulture);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_WithMixedContent_EscapesOnlySpecialCharacters()
    {
        var result = this.renderer.ToString(new SafeMarkdown("Team *A* (Church_1)"), null!, CultureInfo.InvariantCulture);

        Assert.Equal(@"Team \*A\* \(Church\_1\)", result);
    }

    [Fact]
    public void ToString_WithNoSpecialCharacters_ReturnsValueUnchanged()
    {
        var result = this.renderer.ToString(new SafeMarkdown("Team A"), null!, CultureInfo.InvariantCulture);

        Assert.Equal("Team A", result);
    }
}
