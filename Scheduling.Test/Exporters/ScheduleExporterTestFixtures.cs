namespace Scheduling.Test.Exporters;

using System;
using System.Collections.Generic;
using System.IO;

using MatchMaker.Models;
using MatchMaker.Scheduling.Tournaments;

/// <summary>
/// Shared fixture helpers for the schedule exporter tests.
/// </summary>
internal static class ScheduleExporterTestFixtures
{
    /// <summary>
    /// Creates a fully populated <see cref="Schedule"/> with rounds produced by a round-robin tournament.
    /// </summary>
    /// <param name="name">The schedule name</param>
    /// <param name="numberOfTeams">The number of teams to create</param>
    /// <param name="startDate">The optional fixed date assigned to every round; defaults to the current date</param>
    /// <returns>The <see cref="Schedule"/></returns>
    public static Schedule CreateTestSchedule(string name, int numberOfTeams = 4, DateOnly? startDate = null)
    {
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };

        var teams = new Dictionary<int, Team>();
        var quizzers = new Dictionary<int, Quizzer>();
        for (var i = 1; i <= numberOfTeams; i++)
        {
            teams.Add(i, new Team(i, $"Team {i}", $"T{i}", 0));
            quizzers.Add(i, new Quizzer(i, $"First{i}", $"Last{i}", Gender.Female, DateTime.Now.Year, i, 1));
        }

        var schedule = new Schedule(name, churches, quizzers, teams, new Dictionary<int, Round>());
        return RoundRobinTournament.Create(schedule, numberOfTeams / 2, startDate);
    }

    /// <summary>
    /// Creates a <see cref="Schedule"/> with teams but no rounds/matches.
    /// </summary>
    /// <param name="name">The schedule name</param>
    /// <returns>The <see cref="Schedule"/></returns>
    public static Schedule CreateEmptyTestSchedule(string name)
    {
        var churches = new Dictionary<int, Church> { { 1, new Church(1, "Church 1") } };
        var teams = new Dictionary<int, Team> { { 1, new Team(1, "Team 1", "T1", 0) } };
        var quizzers = new Dictionary<int, Quizzer>
        {
            { 1, new Quizzer(1, "First1", "Last1", Gender.Female, DateTime.Now.Year, 1, 1) }
        };

        return new Schedule(name, churches, quizzers, teams, new Dictionary<int, Round>());
    }

    /// <summary>
    /// Creates a unique temporary directory.
    /// </summary>
    /// <returns>The path of the created directory</returns>
    public static string CreateTempDirectory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        return tempDir;
    }

    /// <summary>
    /// Deletes the specified directory if it exists.
    /// </summary>
    /// <param name="directory">The directory to delete</param>
    public static void CleanupDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}
