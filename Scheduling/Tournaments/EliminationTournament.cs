namespace MatchMaker.Scheduling.Tournaments;

using System;
using System.Collections.Generic;
using System.Linq;

using MatchMaker.Models;

/// <summary>
/// Creates single-elimination tournament schedules for a fixed set of seeded teams.
/// </summary>
public static class EliminationTournament
{
    /// <summary>
    /// Creates a single-elimination tournament schedule containing only the given seeded teams
    /// and their quizzers, with a first round pairing the strongest remaining seed against the
    /// weakest remaining seed (for example, seed 1 versus seed 4, and seed 2 versus seed 3).
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
    /// <remarks>
    /// When an odd number of teams is given, the strongest seed receives a bye in the first round;
    /// the remaining teams are paired using the same strongest-versus-weakest pattern.
    /// </remarks>
    public static Schedule Create(
        Schedule schedule,
        IReadOnlyList<int> seededTeamIds,
        DateOnly? startDate = null,
        TimeOnly? startTime = null)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        ArgumentNullException.ThrowIfNull(seededTeamIds);

        if (seededTeamIds.Count < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(seededTeamIds), seededTeamIds.Count, "At least two teams are required to create an elimination tournament.");
        }

        var teams = seededTeamIds.ToDictionary(id => id, id => schedule.Teams[id]);
        var quizzers = schedule.Quizzers
            .Where(x => teams.ContainsKey(x.Value.TeamId))
            .ToDictionary(x => x.Key, x => x.Value);

        var round = CreateFirstRound(seededTeamIds, startDate, startTime);

        return new Schedule(schedule.Name, schedule.Churches, quizzers, teams, new Dictionary<int, Round> { { round.Id, round } });
    }

    /// <summary>
    /// Creates the first round of the elimination tournament.
    /// </summary>
    /// <param name="seededTeamIds">The seeded team identifiers, strongest seed first.</param>
    /// <param name="startDate">The optional round date.</param>
    /// <param name="startTime">The optional round start time.</param>
    /// <returns>The first <see cref="Round"/> instance.</returns>
    private static Round CreateFirstRound(IReadOnlyList<int> seededTeamIds, DateOnly? startDate, TimeOnly? startTime)
    {
        // For an odd number of teams the strongest seed sits out the first round; the remaining
        // teams are paired using the same strongest-versus-weakest pattern.
        var pairingIds = seededTeamIds.Count % 2 == 0 ? seededTeamIds : seededTeamIds.Skip(1).ToList();

        var now = DateTime.Now;
        var date = startDate ?? DateOnly.FromDateTime(now);
        var time = startTime ?? TimeOnly.FromDateTime(now);

        var matches = new Dictionary<int, MatchSchedule>();
        var matchCount = pairingIds.Count / 2;

        for (var i = 0; i < matchCount; i++)
        {
            var team1Id = pairingIds[i];
            var team2Id = pairingIds[pairingIds.Count - 1 - i];
            var matchId = i + 1;
            matches.Add(matchId, new MatchSchedule(matchId, matchId, new[] { team1Id, team2Id }));
        }

        return new Round(1, matches, date, time);
    }
}
