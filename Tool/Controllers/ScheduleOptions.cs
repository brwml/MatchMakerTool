namespace MatchMaker.Tool.Controllers;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ScheduleOptions" />
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ScheduleOptions"/> class.
/// </remarks>
/// <param name="outputSchedule">The output schedule.</param>
/// <param name="rooms">The rooms.</param>
/// <param name="scheduleType">Type of the schedule.</param>
/// <param name="sourceSchedule">The source schedule.</param>
/// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
[Verb("schedule", HelpText = "Generates a tournament schedule")]
internal class ScheduleOptions(
    string inputSchedulePath,
    string outputSchedulePath,
    int rooms,
    ScheduleType scheduleType,
    bool verbose) : BaseOptions(verbose)
{
    /// <summary>
    /// Gets or sets the source schedule file path
    /// </summary>
    [Option('i', Required = true, HelpText = "The source schedule file path")]
    public string InputSchedulePath { get; } = inputSchedulePath;

    /// <summary>
    /// Gets or sets the output schedule file path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output schedule file path")]
    public string OutputSchedulePath { get; } = outputSchedulePath;

    /// <summary>
    /// Gets or sets the rooms available
    /// </summary>
    [Option('n', Required = false, HelpText = "The number of rooms available")]
    public int Rooms { get; } = rooms;

    /// <summary>
    /// Gets or sets the schedule type
    /// </summary>
    [Option('t', Default = ScheduleType.RoundRobin, Required = false, HelpText = "The schedule type. The only option is RoundRobin.")]
    public ScheduleType ScheduleType { get; } = scheduleType;
}
