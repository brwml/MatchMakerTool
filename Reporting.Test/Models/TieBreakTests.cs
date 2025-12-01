namespace Reporting.Test.Models;

using MatchMaker.Models;

using Xunit;

public class TieBreakTests
{
    [Fact]
    public void TieBreak_WithReason_ReturnsHumanizedString()
    {
        var tieBreak = new TieBreak(TieBreakReason.HeadToHead);

        var result = tieBreak.ToString();

        Assert.NotEmpty(result);
        Assert.NotEqual(string.Empty, result);
    }

    [Fact]
    public void TieBreak_NoneReason_ReturnsEmptyString()
    {
        var result = TieBreak.None.ToString();

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void TieBreak_Reason_CorrectlySet()
    {
        var tieBreak = new TieBreak(TieBreakReason.AverageScore);

        Assert.Equal(TieBreakReason.AverageScore, tieBreak.Reason);
    }

    [Theory]
    [InlineData(TieBreakReason.HeadToHead)]
    [InlineData(TieBreakReason.AverageErrors)]
    [InlineData(TieBreakReason.AverageScore)]
    public void TieBreak_VariousReasons_SetCorrectly(TieBreakReason reason)
    {
        var tieBreak = new TieBreak(reason);

        Assert.Equal(reason, tieBreak.Reason);
    }

    [Fact]
    public void TieBreak_NoneInstance_IsConsistent()
    {
        var none1 = TieBreak.None;
        var none2 = TieBreak.None;

        Assert.Same(none1, none2);
    }
}
