namespace MatchMaker.Scheduling.Tournaments;

using MatchMaker.Models;

/// <summary>
/// Defines the ongoing round-by-round lifecycle of a tournament, independent of its specific
/// <see cref="TournamentType"/>. Callers that manage a tournament in progress (for example, a
/// live tournament controller) can depend on this abstraction and use
/// <see cref="TournamentRoundGeneratorFactory.For(TournamentType)"/> to obtain the correct
/// implementation for a <see cref="Schedule"/>, without branching on its type themselves.
/// </summary>
/// <remarks>
/// This interface covers only the lifecycle of rounds after a schedule has already been created;
/// initial schedule creation (for example, <see cref="RoundRobinTournament.Create"/> or
/// <see cref="EliminationTournament.Create"/>) requires format-specific setup data (room counts,
/// seed order, and so on) and is intentionally not part of this abstraction.
/// </remarks>
public interface ITournamentRoundGenerator
{
    /// <summary>
    /// Determines whether round generation for the tournament is complete, that is, whether no
    /// further rounds need to be generated. This reflects the *scheduling* lifecycle only: for
    /// formats whose rounds are all generated up front (for example round robin), this is always
    /// <see langword="true"/> regardless of whether all matches have been played; it does not
    /// determine whether final standings can yet be computed from <paramref name="result"/>.
    /// </summary>
    /// <param name="schedule">The tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns><see langword="true"/> when no further rounds need to be generated; otherwise <see langword="false"/>.</returns>
    bool IsComplete(Schedule schedule, Result result);

    /// <summary>
    /// Creates and appends the next round to the schedule, when one can be determined from the
    /// recorded results.
    /// </summary>
    /// <param name="schedule">The tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the tournament is
    /// already complete (see <see cref="IsComplete"/>), or when the latest round has not yet been
    /// fully resolved and there is not enough information to create the next round.
    /// </returns>
    Round? CreateNextRound(Schedule schedule, Result result);
}
