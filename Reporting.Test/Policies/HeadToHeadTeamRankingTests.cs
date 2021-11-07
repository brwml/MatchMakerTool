namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Xml.Linq;

using MatchMaker.Reporting.Models;
using MatchMaker.Reporting.Policies;

using Xunit;

/// <summary>
/// Defines the <see cref="HeadToHeadTeamRankingTests" />
/// </summary>
public class HeadToHeadTeamRankingTests
{
    [Theory]
    [MemberData(nameof(GetHeadToHeadTestCases))]
    public void HeadToHeadTests(string testId, IDictionary<int, int> places)
    {
        var summary = LoadSummary(@".\Policies\Data\Participants.xml", @$".\Policies\Data\Schedule.{testId}.xml", @$".\Policies\Data\Results.{testId}.xml");
        Assert.All(summary.TeamSummaries, x => Assert.Equal(x.Value.Place, places[x.Key]));
    }

    public static Summary LoadSummary(string participantsPath, string schedulePath, string resultsPath)
    {
        var scheduleXml = XElement.Load(participantsPath);
        scheduleXml.Add(XElement.Load(schedulePath));
        var schedule = Schedule.FromXml(new XDocument(scheduleXml), "Head-To-Head Test");
        var result = Result.FromXml(new[] { XDocument.Load(resultsPath) }, schedule);
        return Summary.FromResult(result, new[] { new HeadToHeadTeamRankingPolicy() });
    }

    public static IEnumerable<object[]> GetHeadToHeadTestCases()
    {
        return new List<object[]>
        {
            new object[]{ "0002", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 } } },
            new object[]{ "0003", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 } } },
            new object[]{ "0007", new Dictionary<int, int>{ { 1, 1 }, { 2, 3 }, { 3, 1 } } },
            new object[]{ "0008", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 }, { 3, 2 } } },
            new object[]{ "0015", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 } } },
            new object[]{ "0019", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } } },
            new object[]{ "0035", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 6 }, { 7, 6 }, { 8, 8 } } },
        };
    }
}
