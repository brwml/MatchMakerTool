namespace MatchMaker.Reporting.Test.Policies
{
    using System.Collections.Generic;
    using System.IO;

    using MatchMaker.Reporting.Models;
    using MatchMaker.Reporting.Policies;

    using Newtonsoft.Json;

    using Xunit;

    /// <summary>
    /// Defines the <see cref="HeadToHeadTeamRankingTests" />
    /// </summary>
    public class HeadToHeadTeamRankingTests
    {
        [Theory]
        [MemberData(nameof(GetHeadToHeadTestCases))]
        public void HeadToHeadTests(string fileName, IDictionary<int, int> places)
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(fileName));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.All(summary.TeamSummaries, x => Assert.Equal(x.Value.Place, places[x.Key]));
        }

        public static IEnumerable<object[]> GetHeadToHeadTestCases()
        {
            return new List<object[]>
            {
                new object[]{ @".\Policies\Data\results.0015.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 } } },
                new object[]{ @".\Policies\Data\results.0019.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 } } },
                new object[]{ @".\Policies\Data\results.0002.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 } } },
                new object[]{ @".\Policies\Data\results.0003.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 } } },
                new object[]{ @".\Policies\Data\results.0035.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 1 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 6 }, { 7, 6 }, { 8, 8 } } },
                new object[]{ @".\Policies\Data\results.0007.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 3 }, { 3, 1 } } },
                new object[]{ @".\Policies\Data\results.0008.json", new Dictionary<int, int>{ { 1, 1 }, { 2, 2 }, { 3, 2 } } },
            };
        }
    }
}
