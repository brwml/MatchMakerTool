namespace Reporting.Test.Policies;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

using MatchMaker.Models;
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
    [SuppressMessage("Usage", "xUnit1045:Avoid using TheoryData type arguments that might not be serializable", Justification = "<Pending>")]
    public void HeadToHeadTests(string testId, IDictionary<int, int> places)
    {
        var summary = LoadSummary(
            @".\Policies\Data\Participants.xml",
            FormattableString.Invariant(@$".\Policies\Data\Schedule.{testId}.xml"),
            FormattableString.Invariant(@$".\Policies\Data\Results.{testId}.xml"));
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

    public static TheoryData<string, IDictionary<int, int>> GetHeadToHeadTestCases()
    {
        return new TheoryData<string, IDictionary<int, int>>
        {
            { "0002", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 } } },
            { "0003", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 } } },
            { "0007", new Dictionary<int, int>{ { 1, 1 }, { 2, 3 }, { 3, 1 } } },
            { "0008", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 }, { 3, 2 } } },
            { "0015", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 } } },
            { "0019", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } } },
            { "0035", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 6 }, { 7, 6 }, { 8, 8 } } },
        };
    }
}
