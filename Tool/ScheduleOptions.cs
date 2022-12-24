namespace MatchMaker.Tool;

using CommandLine;

#pragma warning disable CS8618 // Nullable types are either required or have default values.
#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ScheduleOptions" />
/// </summary>
[Verb("schedule", HelpText = "Generates a tournament schedule")]
internal class ScheduleOptions : BaseOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the Swiss schedule is created a priori.
    /// </summary>
    [Option('a', Default = true, Required = false, HelpText = "Indicates whether the schedule is created a priori", SetName = "Swiss")]
    public bool IsApriori { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tournament is seeded
    /// </summary>
    [Option('s', Default = true, Required = false, HelpText = "Indicates whether the tournament is seeded")]
    public bool IsSeeded { get; set; }

    /// <summary>
    /// Gets or sets the output schedule file path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output schedule file path")]
    public string OutputSchedule { get; set; }

    /// <summary>
    /// Gets or sets the results folder for a Swiss tournament
    /// </summary>
    [Option('r', Required = false, HelpText = "The results folder", SetName = "Swiss")]
    public string ResultsFolder { get; set; }

    /// <summary>
    /// Gets or sets the rooms available
    /// </summary>
    [Option('n', Required = false, HelpText = "The number of rooms available")]
    public int Rooms { get; set; }

    /// <summary>
    /// Gets or sets the schedule type
    /// </summary>
    [Option('t', Default = ScheduleType.RoundRobin, Required = false, HelpText = "The schedule type. Options include RoundRobin and Swiss.")]
    public ScheduleType ScheduleType { get; set; }

    /// <summary>
    /// Gets or sets the source schedule file path
    /// </summary>
    [Option('i', Required = true, HelpText = "The source schedule file path")]
    public string SourceSchedule { get; set; }
}
