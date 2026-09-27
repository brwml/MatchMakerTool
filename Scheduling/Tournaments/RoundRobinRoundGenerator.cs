namespace MatchMaker.Scheduling.Tournaments;

using System;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for round-robin tournaments, whose rounds
/// are all generated up front by <see cref="RoundRobinTournament.Create"/>; there is never a
/// further round to generate dynamically from results.
/// </summary>
public sealed class RoundRobinRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        return true;
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(result);

        return null;
    }
}
