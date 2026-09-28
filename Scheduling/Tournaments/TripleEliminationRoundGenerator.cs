namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for triple-elimination tournaments by
/// delegating to <see cref="TripleEliminationTournament.IsComplete"/> and
/// <see cref="TripleEliminationTournament.AdvanceRound"/>.
/// </summary>
public sealed class TripleEliminationRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        return TripleEliminationTournament.IsComplete(schedule, result);
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        return TripleEliminationTournament.AdvanceRound(schedule, result);
    }
}
