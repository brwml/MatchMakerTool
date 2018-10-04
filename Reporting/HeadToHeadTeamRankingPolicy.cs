using System;
using System.Collections.Generic;
using System.Linq;

using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Condensation;

namespace MatchMaker.Reporting
{
    using DependencyGraph = AdjacencyGraph<int, Edge<int>>;
    using GraphEdge = Edge<int>;
    using GraphVertex = Int32;

    public class HeadToHeadTeamRankingPolicy : TeamRankingPolicy
    {
        protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
        {
            this.RankGroupInternal(summaries, initial);
        }

        private static IMutableBidirectionalGraph<DependencyGraph, CondensedEdge<int, GraphEdge, DependencyGraph>> CondensateGraph(DependencyGraph graph)
        {
            return graph.CondensateStronglyConnected<GraphVertex, GraphEdge, DependencyGraph>();
        }

        private static List<List<DependencyGraph>> ResolveIndeterminantOrderings(IMutableBidirectionalGraph<DependencyGraph, CondensedEdge<GraphVertex, GraphEdge, DependencyGraph>> condensated, List<List<DependencyGraph>> ordered)
        {
            var position = 0;

            while (position < ordered.Count - 1)
            {
                var paths = ordered[position].Select(x => condensated.ShortestPathsDijkstra(e => 1, x));

                if (ordered[position + 1].All(x => paths.All(p => p(x, out var path))))
                {
                    position++;
                }
                else
                {
                    ordered[position] = ordered[position].Concat(ordered[position + 1]).ToList();
                    ordered.RemoveAt(position + 1);
                    position = 0;
                }
            }

            return ordered;
        }

        private static List<List<DependencyGraph>> SortCondensatedGraph(IMutableBidirectionalGraph<DependencyGraph, CondensedEdge<GraphVertex, GraphEdge, DependencyGraph>> condensated)
        {
            return condensated.TopologicalSort().Select(x => new List<DependencyGraph> { x }).ToList();
        }

        private IEnumerable<MatchResult> GetMatchesForTeamSummaries(IEnumerable<TeamSummary> summaries)
        {
            return this.Result.Matches.Select(m => m.Value).Where(m => m.TeamResults.All(t => summaries.Any(s => s.TeamId == t.TeamId)));
        }

        private DependencyGraph InitializeDependencyGraph(IEnumerable<TeamSummary> summaries)
        {
            var graph = new DependencyGraph();
            graph.AddVertexRange(summaries.Select(x => x.TeamId));
            var matches = this.GetMatchesForTeamSummaries(summaries);
            graph.AddEdgeRange(matches.Select(m => new GraphEdge(m.TeamResults.First(x => x.Place == 1).TeamId, m.TeamResults.First(x => x.Place == 2).TeamId)));

            return graph;
        }

        private void RankGroupInternal(IEnumerable<TeamSummary> summaries, int initial)
        {
            var graph = this.InitializeDependencyGraph(summaries);
            var condensated = CondensateGraph(graph);
            var ordered = SortCondensatedGraph(condensated);
            ordered = ResolveIndeterminantOrderings(condensated, ordered);

            this.SetRelativePlaces(summaries, initial, ordered);
        }

        private void SetRelativePlaces(IEnumerable<TeamSummary> summaries, int initial, List<List<DependencyGraph>> ordered)
        {
            var place = initial;
            var summaryLookup = summaries.ToDictionary(x => x.TeamId, x => x);
            var matches = this.GetMatchesForTeamSummaries(summaries);

            foreach (var group in ordered)
            {
                var teams = group.SelectMany(x => x.Vertices).ToArray();

                foreach (var team in teams)
                {
                    var teamSummary = summaryLookup[team];
                    teamSummary.Place = place;
                    summaryLookup[team].TieBreak = new TieBreakHeadToHead(matches.Where(x => x.TeamResults.Any(y => y.TeamId == teamSummary.TeamId)), this.Result.Schedule.Teams);
                }

                place += teams.Length;
            }
        }
    }
}