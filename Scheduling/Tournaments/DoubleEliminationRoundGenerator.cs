namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for double-elimination tournaments by
/// delegating to <see cref="DoubleEliminationTournament.IsComplete"/> and
/// <see cref="DoubleEliminationTournament.AdvanceRound"/>.
/// </summary>
public sealed class DoubleEliminationRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        return DoubleEliminationTournament.IsComplete(schedule, result);
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        return DoubleEliminationTournament.AdvanceRound(schedule, result);
    }
}
