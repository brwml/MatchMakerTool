namespace MatchMaker.Reporting.Policies;

using System;
using System.Collections.Generic;
using System.Linq;

using Ardalis.GuardClauses;

using MatchMaker.Reporting.Models;

/// <summary>
/// Defines the <see cref="QuizzerRankingPolicy" />
/// </summary>
public abstract class QuizzerRankingPolicy
{
    /// <summary>
    /// Ranks the quizzers.
    /// </summary>
    /// <param name="summaries">The <see cref="QuizzerSummary"/> instances</param>
    public void Rank(IEnumerable<QuizzerSummary> summaries)
    {
        Guard.Against.NullOrEmpty(summaries, nameof(summaries));

        var groups = summaries.GroupBy(s => s.Place);

        foreach (var group in groups)
        {
            this.RankGroup(group, group.Min(g => g.Place));
        }
    }

    /// <summary>
    /// Sets the relative places for each quizzer group.
    /// </summary>
    /// <param name="summaries">The <see cref="QuizzerSummary"/> instances</param>
    /// <param name="initial">The initial placement</param>
    /// <param name="areEqual">A <see cref="Func{QuizzerSummary, QuizzerSummary, Boolean}"/> indicating whether two placements are equal.</param>
    protected static void SetRelativePlaces(IList<QuizzerSummary> summaries, int initial, Func<QuizzerSummary, QuizzerSummary, bool> areEqual)
    {
        if (summaries is null || areEqual is null)
        {
            return;
        }

        for (var i = 1; i < summaries.Count; i++)
        {
            if (areEqual(summaries[i], summaries[i - 1]))
            {
                summaries[i].Place = summaries[i - 1].Place;
            }
            else
            {
                summaries[i].Place = initial + i;
            }
        }
    }

    /// <summary>
    /// An abstract method that ranks the <see cref="QuizzerSummary"/> instances by some policy.
    /// </summary>
    /// <param name="summaries">The <see cref="QuizzerSummary"/> instances</param>
    /// <param name="initial">The initial placement</param>
    protected abstract void RankGroup(IEnumerable<QuizzerSummary> summaries, int initial);
}
