namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Implements <see cref="ITournamentRoundGenerator"/> for Swiss-with-top-cut tournaments by
/// delegating to <see cref="SwissWithTopCutTournament.IsComplete"/> and
/// <see cref="SwissWithTopCutTournament.AdvanceRound"/>.
/// </summary>
public sealed class SwissWithTopCutRoundGenerator : ITournamentRoundGenerator
{
    /// <inheritdoc />
    public bool IsComplete(Schedule schedule, Result result)
    {
        return SwissWithTopCutTournament.IsComplete(schedule, result);
    }

    /// <inheritdoc />
    public Round? CreateNextRound(Schedule schedule, Result result)
    {
        return SwissWithTopCutTournament.AdvanceRound(schedule, result);
    }
}
