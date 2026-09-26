namespace MatchMaker.Tool.Controllers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using MatchMaker.Models;
using MatchMaker.Reporting.Exporters;
using MatchMaker.Reporting.Models;
using MatchMaker.Scheduling.Teams;
using MatchMaker.Scheduling.Tournaments;

/// <summary>
/// Creates the elimination tournament and alternate team artifacts from a tournament
/// <see cref="Summary"/>.
/// </summary>
internal static class TournamentTeamsProcessor
{
    /// <summary>
    /// Suffix appended to the tournament name for the elimination tournament schedule.
    /// </summary>
    private const string EliminationSuffix = "-Elimination";

    /// <summary>
    /// Suffix appended to the tournament name for the alternate teams schedule and roster.
    /// </summary>
    private const string AlternateSuffix = "-Alternative";

    /// <summary>
    /// Validates that the tournament team options are consistent with the tournament summary
    /// before any artifacts are generated.
    /// </summary>
    /// <param name="summary">The tournament summary.</param>
    /// <param name="numberOfTournamentTeams">The number of top-placing teams that advance to the elimination tournament.</param>
    /// <param name="numberOfAlternateTeams">The number of alternate teams to create from the remaining quizzers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="summary"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="numberOfTournamentTeams"/> is less than 2 or greater than or
    /// equal to the total number of teams, or when <paramref name="numberOfAlternateTeams"/> is
    /// less than 1.
    /// </exception>
    public static void Validate(Summary summary, int numberOfTournamentTeams, int numberOfAlternateTeams)
    {
        ArgumentNullException.ThrowIfNull(summary);

        var totalTeams = summary.TeamSummaries.Count;

        if (numberOfTournamentTeams < 2 || numberOfTournamentTeams >= totalTeams)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numberOfTournamentTeams),
                numberOfTournamentTeams,
                $"The number of tournament teams must be at least 2 and less than the total number of teams ({totalTeams}).");
        }

        if (numberOfAlternateTeams < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfAlternateTeams), numberOfAlternateTeams, "The number of alternate teams must be at least 1.");
        }
    }

    /// <summary>
    /// Creates the elimination tournament schedule for the top-placing teams and the alternate
    /// teams round-robin schedule and roster for the remaining quizzers.
    /// </summary>
    /// <param name="summary">The tournament summary.</param>
    /// <param name="numberOfTournamentTeams">The number of top-placing teams that advance to the elimination tournament.</param>
    /// <param name="numberOfAlternateTeams">The number of alternate teams to create from the remaining quizzers.</param>
    /// <param name="numberOfAlternateRooms">
    /// The number of rooms available for the alternate teams round-robin tournament. When less
    /// than 1, every match in a round is scheduled to run simultaneously.
    /// </param>
    /// <param name="outputFolder">The output folder.</param>
    public static void Process(
        Summary summary,
        int numberOfTournamentTeams,
        int numberOfAlternateTeams,
        int numberOfAlternateRooms,
        string outputFolder)
    {
        Validate(summary, numberOfTournamentTeams, numberOfAlternateTeams);

        // Set (or re-affirm) the elimination team count so `Summary.EliminationTeamIds` reflects
        // it, even if a caller invokes this method without having already set it on the summary
        // (the reporting controller sets it earlier so team reports can reference it too).
        summary.NumberOfEliminationTeams = numberOfTournamentTeams;

        Trace.WriteLine($"Creating tournament team artifacts for '{summary.Name}'");
        Trace.Indent();

        try
        {
            var schedule = summary.Result.Schedule;

            var eliminationTeamIds = summary.EliminationTeamIds;
            var eliminationSchedule = EliminationTournament.Create(schedule, eliminationTeamIds).WithName(summary.Name + EliminationSuffix);
            WriteScheduleXml(eliminationSchedule, outputFolder);

            var alternateQuizzerIds = GetAlternateQuizzerIds(schedule, summary, eliminationTeamIds);
            var alternateSchedule = BalancedTeamAssigner.Create(schedule, alternateQuizzerIds, numberOfAlternateTeams);
            var availableRooms = numberOfAlternateRooms > 0 ? numberOfAlternateRooms : numberOfAlternateTeams;
            alternateSchedule = RoundRobinTournament.Create(alternateSchedule, availableRooms).WithName(summary.Name + AlternateSuffix);
            WriteScheduleXml(alternateSchedule, outputFolder);

            AlternateTeamsExcelExporter.Export(alternateSchedule, outputFolder);

            Trace.WriteLine("Tournament team artifacts created successfully");
        }
        finally
        {
            Trace.Unindent();
        }
    }

    /// <summary>
    /// Gets the identifiers of every quizzer on the roster whose team did not qualify for the
    /// elimination tournament, ordered from the best-placing quizzer to the worst-placing
    /// quizzer. Roster members with no individual results (and therefore no <see
    /// cref="QuizzerSummary"/>) are included, ordered after every ranked quizzer, by name.
    /// </summary>
    /// <param name="schedule">The full tournament schedule.</param>
    /// <param name="summary">The tournament summary.</param>
    /// <param name="eliminationTeamIds">The identifiers of the teams that qualified for the elimination tournament.</param>
    /// <returns>The ordered list of alternate quizzer identifiers.</returns>
    private static List<int> GetAlternateQuizzerIds(Schedule schedule, Summary summary, IReadOnlyList<int> eliminationTeamIds)
    {
        var eliminationTeams = new HashSet<int>(eliminationTeamIds);
        var quizzerSummaries = summary.QuizzerSummaries;

        return schedule.Quizzers.Values
            .Where(x => !eliminationTeams.Contains(x.TeamId))
            .OrderBy(x => quizzerSummaries.TryGetValue(x.Id, out var s) ? s.Place : int.MaxValue)
            .ThenBy(x => x.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.FirstName, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Id)
            .ToList();
    }

    /// <summary>
    /// Writes the given schedule to a "{name}.schedule.xml" file so it can be reused as the input
    /// for a subsequent tournament.
    /// </summary>
    /// <param name="schedule">The schedule to write.</param>
    /// <param name="folder">The output folder.</param>
    private static void WriteScheduleXml(Schedule schedule, string folder)
    {
        var path = Path.Combine(folder, FormattableString.Invariant($"{schedule.Name}.schedule.xml"));
        Trace.WriteLine($"Writing schedule XML to: {path}");
        schedule.ToXml().Save(path);
    }
}
