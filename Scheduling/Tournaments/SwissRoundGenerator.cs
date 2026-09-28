namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for Swiss-system tournaments by delegating
/// to <see cref="SwissTournament.IsComplete"/> and <see cref="SwissTournament.AdvanceRound"/>.
/// </summary>
public sealed class SwissRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        return SwissTournament.IsComplete(schedule, result);
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        return SwissTournament.AdvanceRound(schedule, result);
    }
}
