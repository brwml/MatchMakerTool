namespace Reporting.Test.Exporters;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;

using Xunit;

public class TeamInfoTests
{
    [Fact]
    public void TeamInfo_WithoutIsEliminationArgument_DefaultsToFalse()
    {
        var team = new Team(1, "Team 1", "T1", 0);
        var summary = new TeamSummary { TeamId = 1, Wins = 1, Losses = 0 };

        var teamInfo = new TeamInfo(team, summary);

        Assert.False(teamInfo.IsElimination);
    }

    [Fact]
    public void TeamInfo_WithIsEliminationTrue_SetsIsElimination()
    {
        var team = new Team(1, "Team 1", "T1", 0);
        var summary = new TeamSummary { TeamId = 1, Wins = 1, Losses = 0 };

        var teamInfo = new TeamInfo(team, summary, true);

        Assert.True(teamInfo.IsElimination);
    }
}
