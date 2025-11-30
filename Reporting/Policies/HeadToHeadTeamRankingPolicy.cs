namespace MatchMaker.Reporting.Policies;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Condensation;

using DependencyGraph = QuikGraph.AdjacencyGraph<int, QuikGraph.Edge<int>>;
using GraphEdge = QuikGraph.Edge<int>;
using GraphVertex = Int32;

/// <summary>
/// Defines the <see cref="HeadToHeadTeamRankingPolicy" />
/// </summary>
public class HeadToHeadTeamRankingPolicy : TeamRankingPolicy
{
    /// <summary>
    /// Ranks a collection of <see cref="TeamSummary"/> instances.
    /// </summary>
    /// <param name="summaries">The summaries</param>
    /// <param name="initial">The initial</param>
    protected override void RankGroup(IEnumerable<TeamSummary> summaries, int initial)
    {
        Trace.WriteLine("Ranking teams by head-to-head competition");
        this.RankGroupInternal(summaries, initial);
    }

    /// <summary>
    /// Condensates the given <see cref="DependencyGraph"/> instance
    /// </summary>
    /// <param name="graph">The condensated graph</param>
    /// <returns>The <see cref="IMutableBidirectionalGraph{DependencyGraph, CondensedEdge{int, GraphEdge, DependencyGraph}}"/> instance</returns>
    private static IMutableBidirectionalGraph<DependencyGraph, CondensedEdge<int, GraphEdge, DependencyGraph>> CondensateGraph(DependencyGraph graph)
    {
        return graph.CondensateStronglyConnected<GraphVertex, GraphEdge, DependencyGraph>();
    }

    /// <summary>
    /// Resolves indeterminant orderings. An ordering is indeterminant when a path does not exist
    /// between all members of one position and the next. When this occurs the two positions are
    /// combined into a single position so that the two remain tied.
    /// </summary>
    /// <param name="condensated">The condensated graph</param>
    /// <param name="ordered">The ordered dependency graph</param>
    /// <returns>The <see cref="List{List{DependencyGraph}}"/> instance</returns>
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
                ordered[position] = [.. ordered[position], .. ordered[position + 1]];
                ordered.RemoveAt(position + 1);
                position = 0;
            }
        }

        return ordered;
    }

    /// <summary>
    /// Sorts the condensated graph
    /// </summary>
    /// <param name="condensated">The condensated graph</param>
    /// <returns>The ordered <see cref="List{List{DependencyGraph}}"/> instance</returns>
    private static List<List<DependencyGraph>> SortCondensatedGraph(IMutableBidirectionalGraph<DependencyGraph, CondensedEdge<GraphVertex, GraphEdge, DependencyGraph>> condensated)
    {
        return condensated.TopologicalSort().Select(x => new List<DependencyGraph> { x }).ToList();
    }

    /// <summary>
    /// Gets the <see cref="TeamSummary"/> matches.
    /// </summary>
    /// <param name="summaries">The <see cref="TeamSummary"/> instances</param>
    /// <returns>The <see cref="IEnumerable{MatchResult}"/> instance</returns>
    private IEnumerable<MatchResult> GetMatchesForTeamSummaries(IEnumerable<TeamSummary> summaries)
    {
        return this.Result?.Matches.Select(m => m.Value).Where(m => m.TeamResults.All(t => summaries.Any(s => s.TeamId == t.TeamId))) ?? [];
    }

    /// <summary>
    /// Initializes the <see cref="DependencyGraph"/> instance
    /// </summary>
    /// <param name="summaries">The <see cref="TeamSummary"/> instances</param>
    /// <returns>The <see cref="DependencyGraph"/> instance</returns>
    private DependencyGraph InitializeDependencyGraph(IEnumerable<TeamSummary> summaries)
    {
        var graph = new DependencyGraph();
        graph.AddVertexRange(summaries.Select(x => x.TeamId));
        var matches = this.GetMatchesForTeamSummaries(summaries);
        graph.AddEdgeRange(matches.Select(m => new GraphEdge(m.TeamResults.First(x => x.Place == 1).TeamId, m.TeamResults.First(x => x.Place == 2).TeamId)));

        return graph;
    }

    /// <summary>
    /// Ranks the collection of <see cref="TeamSummary"/> instances
    /// </summary>
    /// <param name="summaries">The <see cref="TeamSummary"/> instances</param>
    /// <param name="initial">The initial place</param>
    private void RankGroupInternal(IEnumerable<TeamSummary> summaries, int initial)
    {
        var graph = this.InitializeDependencyGraph(summaries);
        var condensated = CondensateGraph(graph);
        var ordered = SortCondensatedGraph(condensated);
        ordered = ResolveIndeterminantOrderings(condensated, ordered);

        this.SetRelativePlaces(summaries, initial, ordered);
    }

    /// <summary>
    /// Sets the relative placements
    /// </summary>
    /// <param name="summaries">The <see cref="TeamSummary"/> instances</param>
    /// <param name="initial">The initial placement</param>
    /// <param name="ordered">The ordered <see cref="DependencyGraph"/> instances</param>
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
                summaryLookup[team].TieBreak = new TieBreakHeadToHead(matches.Where(x => x.TeamResults.Any(y => y.TeamId == teamSummary.TeamId)), this.Result?.Schedule.Teams ?? new Dictionary<int, Team>());
            }

            place += teams.Length;
        }
    }
}
