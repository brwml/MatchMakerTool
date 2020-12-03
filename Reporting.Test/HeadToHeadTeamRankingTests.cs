namespace MatchMaker.Reporting.Test
{
    using System.IO;

    using MatchMaker.Reporting.Models;
    using MatchMaker.Reporting.Policies;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="HeadToHeadTeamRankingTests" />
    /// </summary>
    [TestClass]
    public class HeadToHeadTeamRankingTests
    {
        /// <summary>
        /// The HeadToHeadCase15
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0015.json", "Data")]
        public void HeadToHeadCase15()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0015.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(1, summary.TeamSummaries[2].Place);
            Assert.AreEqual(1, summary.TeamSummaries[3].Place);
        }

        /// <summary>
        /// The HeadToHeadCase19
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0019.json", "Data")]
        public void HeadToHeadCase19()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0019.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(1, summary.TeamSummaries[2].Place);
            Assert.AreEqual(1, summary.TeamSummaries[3].Place);
            Assert.AreEqual(1, summary.TeamSummaries[4].Place);
        }

        /// <summary>
        /// The HeadToHeadCase2
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0002.json", "Data")]
        public void HeadToHeadCase2()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0002.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(2, summary.TeamSummaries[2].Place);
        }

        /// <summary>
        /// The HeadToHeadCase3
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0003.json", "Data")]
        public void HeadToHeadCase3()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0003.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(1, summary.TeamSummaries[2].Place);
        }

        /// <summary>
        /// The HeadToHeadCase35
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0035.json", "Data")]
        public void HeadToHeadCase35()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0035.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(1, summary.TeamSummaries[2].Place);
            Assert.AreEqual(3, summary.TeamSummaries[3].Place);
            Assert.AreEqual(3, summary.TeamSummaries[4].Place);
            Assert.AreEqual(3, summary.TeamSummaries[5].Place);
            Assert.AreEqual(6, summary.TeamSummaries[6].Place);
            Assert.AreEqual(6, summary.TeamSummaries[7].Place);
            Assert.AreEqual(8, summary.TeamSummaries[8].Place);
        }

        /// <summary>
        /// The HeadToHeadCase7
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0007.json", "Data")]
        public void HeadToHeadCase7()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0007.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(3, summary.TeamSummaries[2].Place);
            Assert.AreEqual(1, summary.TeamSummaries[3].Place);
        }

        /// <summary>
        /// The HeadToHeadCase8
        /// </summary>
        [TestMethod]
        [DeploymentItem(@".\Data\results.0008.json", "Data")]
        public void HeadToHeadCase8()
        {
            var results = JsonConvert.DeserializeObject<Result>(File.ReadAllText(@".\Data\results.0008.json"));
            var summary = Summary.FromResult(results, new[] { new HeadToHeadTeamRankingPolicy() });

            Assert.AreEqual(1, summary.TeamSummaries[1].Place);
            Assert.AreEqual(2, summary.TeamSummaries[2].Place);
            Assert.AreEqual(2, summary.TeamSummaries[3].Place);
        }
    }
}
