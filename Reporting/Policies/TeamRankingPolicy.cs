namespace MatchMaker.Reporting.Policies;

using System;
using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Models;
using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="TeamRankingPolicy" />
/// </summary>
public abstract class TeamRankingPolicy
{
    /// <summary>
    /// Gets or sets the Result
    /// </summary>
    protected Result Result { get; private set; } = Result.Null;

    /// <summary>
    /// Ranks the teams.
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/></param>
    /// <param name="result">The <see cref="Result"/></param>
    public void Rank(IEnumerable<TeamSummary> summaries, Result result)
    {
        Guard.Against.NullOrEmpty(summaries);

        this.Result = result;
        var groups = summaries.GroupBy(s => s.Place);

        foreach (var group in groups)
        {
            if (group.Count() > 1)
            {
                this.RankGroup(group, group.Min(g => g.Place));
            }
        }
    }

    /// <summary>
    /// The SetRelativePlaces
    /// </summary>
    /// <param name="summaries">The <see cref="IList{TeamSummary}"/></param>
    /// <param name="initial">The initial placement</param>
    /// <param name="areEqual">The <see cref="Func{TeamSummary, TeamSummary, bool}"/> that determines whether two <see cref="TeamSummary"/> instances are equal</param>
    /// <param name="tieBreak">The <see cref="TieBreak"/> method that was used.</param>
    protected static void SetRelativePlaces(IList<TeamSummary> summaries, int initial, Func<TeamSummary, TeamSummary, bool> areEqual, TieBreak tieBreak)
    {
        Guard.Against.Null(summaries);
        Guard.Against.NegativeOrZero(initial);
        Guard.Against.Null(areEqual);

        for (var i = 1; i < summaries.Count; i++)
        {
            if (areEqual(summaries[i], summaries[i - 1]))
            {
                summaries[i].Place = summaries[i - 1].Place;
            }
            else
            {
                summaries[i - 1].TieBreak = tieBreak;
                summaries[i].TieBreak = tieBreak;
                summaries[i].Place = initial + i;
            }
        }
    }

    /// <summary>
    /// Ranks a group of teams
    /// </summary>
    /// <param name="summaries">The <see cref="IEnumerable{TeamSummary}"/></param>
    /// <param name="initial">The initial placement</param>
    protected abstract void RankGroup(IEnumerable<TeamSummary> summaries, int initial);
}
