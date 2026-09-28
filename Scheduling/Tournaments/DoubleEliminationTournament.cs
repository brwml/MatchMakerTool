namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;

using MatchMaker.Models;

/// <summary>
/// Creates and advances double-elimination tournament schedules for a fixed set of seeded teams,
/// where a team is eliminated only after its second loss.
/// </summary>
/// <remarks>
/// See the remarks on <see cref="MultiLifeEliminationTournament"/> for how pairings are generated
/// across rounds.
/// </remarks>
public static class DoubleEliminationTournament
{
    /// <summary>
    /// The number of losses a team may accumulate before being eliminated.
    /// </summary>
    private const int Lives = 2;

    /// <summary>
    /// Creates a double-elimination tournament schedule containing only the given seeded teams
    /// and their quizzers, with a first round pairing the strongest remaining seed against the
    /// weakest remaining seed.
    /// </summary>
    /// <param name="schedule">The schedule containing the full list of teams and quizzers.</param>
    /// <param name="seededTeamIds">
    /// The identifiers of the teams that qualify for the elimination tournament, ordered from the
    /// strongest seed to the weakest.
    /// </param>
    /// <param name="startDate">
    /// The optional date assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current date.
    /// </param>
    /// <param name="startTime">
    /// The optional start time assigned to the first round. When <see langword="null"/>, the round
    /// defaults to the current time.
    /// </param>
    /// <returns>The <see cref="Schedule"/> instance containing only the seeded teams, their quizzers, and the first round of matches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="schedule"/> or <paramref name="seededTeamIds"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="seededTeamIds"/> contains fewer than two teams.</exception>
    public static Schedule Create(
        Schedule schedule,
        IReadOnlyList<int> seededTeamIds,
        DateOnly? startDate = null,
        TimeOnly? startTime = null)
    {
        return MultiLifeEliminationTournament.Create(schedule, seededTeamIds, TournamentType.DoubleElimination, startDate, startTime);
    }

    /// <summary>
    /// Determines whether the double-elimination tournament is complete, that is, whether the
    /// latest round has been fully resolved and only a single team (the champion) remains that has
    /// not yet lost twice.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns><see langword="true"/> when the tournament has a decided champion; otherwise <see langword="false"/>.</returns>
    public static bool IsComplete(Schedule schedule, Result result)
    {
        return MultiLifeEliminationTournament.IsComplete(schedule, result, Lives);
    }

    /// <summary>
    /// Creates and appends the next round of the double-elimination tournament from the results of
    /// the latest round, re-pairing every team that has lost fewer than two matches.
    /// </summary>
    /// <param name="schedule">The elimination tournament schedule. The new round, if any, is added directly to <see cref="Schedule.Rounds"/>.</param>
    /// <param name="result">The <see cref="Result"/> containing recorded match outcomes.</param>
    /// <returns>
    /// The newly created <see cref="Round"/>, or <see langword="null"/> when the latest round has
    /// not yet been fully resolved, or when the tournament is already complete.
    /// </returns>
    public static Round? AdvanceRound(Schedule schedule, Result result)
    {
        return MultiLifeEliminationTournament.AdvanceRound(schedule, result, Lives);
    }
}
