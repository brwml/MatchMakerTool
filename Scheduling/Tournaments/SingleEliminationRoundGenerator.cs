namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for single-elimination tournaments by
/// delegating to <see cref="EliminationTournament.IsComplete"/> and
/// <see cref="EliminationTournament.AdvanceRound"/>.
/// </summary>
public sealed class SingleEliminationRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        return EliminationTournament.IsComplete(schedule, result);
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        return EliminationTournament.AdvanceRound(schedule, result);
    }
}
