namespace Reporting.Test.Models;

using System.Collections.Generic;

using MatchMaker.Models;

using Xunit;

public class TieBreakHeadToHeadTests
{
    [Fact]
    public void ToString_ContainsHeadToHeadLabel()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        var text = tieBreak.ToString();

        Assert.Contains("Head To Head", text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToString_ContainsWinnerAbbreviation()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        var text = tieBreak.ToString();

        Assert.Contains(teams[1].Abbreviation, text);
    }

    [Fact]
    public void ToString_ContainsLoserAbbreviation()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        var text = tieBreak.ToString();

        Assert.Contains(teams[2].Abbreviation, text);
    }

    [Fact]
    public void ToString_ShowsWinnerBeforeLoser()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        var text = tieBreak.ToString();
        var winnerIdx = text.IndexOf(teams[1].Abbreviation, StringComparison.Ordinal);
        var loserIdx = text.IndexOf(teams[2].Abbreviation, StringComparison.Ordinal);

        Assert.True(winnerIdx < loserIdx, $"Winner '{teams[1].Abbreviation}' should appear before loser '{teams[2].Abbreviation}' in: {text}");
    }

    [Fact]
    public void ToString_WithMultipleMatches_ListsAllMatchups()
    {
        var teams = new Dictionary<int, Team>
        {
            { 1, new Team(1, "Alpha", "AL", 1) },
            { 2, new Team(2, "Beta", "BT", 1) },
            { 3, new Team(3, "Gamma", "GM", 1) },
        };

        var results = new List<MatchResult>
        {
            BuildMatchResult(id: 101, winnerId: 1, loserId: 2),
            BuildMatchResult(id: 102, winnerId: 3, loserId: 2),
        };

        var tieBreak = new TieBreakHeadToHead(results, teams);
        var text = tieBreak.ToString();

        Assert.Contains("AL", text);
        Assert.Contains("BT", text);
        Assert.Contains("GM", text);
    }

    [Fact]
    public void Reason_IsHeadToHead()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        Assert.Equal(TieBreakReason.HeadToHead, tieBreak.Reason);
    }

    [Fact]
    public void Teams_IsAccessible()
    {
        var (teams, results) = BuildSingleMatch(winnerId: 1, loserId: 2);
        var tieBreak = new TieBreakHeadToHead(results, teams);

        Assert.Equal(2, tieBreak.Teams.Count);
        Assert.Same(teams[1], tieBreak.Teams[1]);
        Assert.Same(teams[2], tieBreak.Teams[2]);
    }

    private static (IDictionary<int, Team> teams, IEnumerable<MatchResult> results) BuildSingleMatch(int winnerId, int loserId)
    {
        var teams = new Dictionary<int, Team>
        {
            { winnerId, new Team(winnerId, $"Team {winnerId}", $"T{winnerId}", 1) },
            { loserId, new Team(loserId, $"Team {loserId}", $"T{loserId}", 1) },
        };

        var results = new List<MatchResult> { BuildMatchResult(101, winnerId, loserId) };

        return (teams, results);
    }

    private static MatchResult BuildMatchResult(int id, int winnerId, int loserId)
    {
        var teamResults = new List<TeamResult>
        {
            new(winnerId, 300, 0, 1),
            new(loserId, 200, 5, 2),
        };

        return new MatchResult(id, 1, 1, teamResults, []);
    }
}
