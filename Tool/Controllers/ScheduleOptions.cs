namespace MatchMaker.Tool.Controllers;

using Ardalis.GuardClauses;

using CommandLine;

#pragma warning disable CA1812 // The class is instantiate by the command line parser.

/// <summary>
/// Defines the <see cref="ScheduleOptions" />
/// </summary>
[Verb("schedule", HelpText = "Generates a tournament schedule")]
internal class ScheduleOptions : BaseOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleOptions"/> class.
    /// </summary>
    /// <param name="outputSchedule">The output schedule.</param>
    /// <param name="rooms">The rooms.</param>
    /// <param name="scheduleType">Type of the schedule.</param>
    /// <param name="sourceSchedule">The source schedule.</param>
    /// <param name="verbose">If set to <c>true</c>, then emit verbose output.</param>
    public ScheduleOptions(
        string inputSchedulePath,
        string outputSchedulePath,
        int rooms,
        ScheduleType scheduleType,
        bool verbose)
        : base(verbose)
    {
        this.InputSchedulePath = Guard.Against.NullOrWhiteSpace(inputSchedulePath);
        this.OutputSchedulePath = Guard.Against.NullOrWhiteSpace(outputSchedulePath);
        this.Rooms = rooms;
        this.ScheduleType = scheduleType;
    }

    /// <summary>
    /// Gets or sets the source schedule file path
    /// </summary>
    [Option('i', Required = true, HelpText = "The source schedule file path")]
    public string InputSchedulePath
    {
        get;
    }

    /// <summary>
    /// Gets or sets the output schedule file path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output schedule file path")]
    public string OutputSchedulePath
    {
        get;
    }

    /// <summary>
    /// Gets or sets the rooms available
    /// </summary>
    [Option('n', Required = false, HelpText = "The number of rooms available")]
    public int Rooms
    {
        get;
    }

    /// <summary>
    /// Gets or sets the schedule type
    /// </summary>
    [Option('t', Default = ScheduleType.RoundRobin, Required = false, HelpText = "The schedule type. The only option is RoundRobin.")]
    public ScheduleType ScheduleType
    {
        get;
    }
}
