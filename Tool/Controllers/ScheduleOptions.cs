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
        string outputSchedule,
        int rooms,
        ScheduleType scheduleType,
        string sourceSchedule,
        bool verbose)
        : base(verbose)
    {
        this.OutputSchedule = Guard.Against.NullOrWhiteSpace(outputSchedule);
        this.Rooms = rooms;
        this.ScheduleType = scheduleType;
        this.SourceSchedule = Guard.Against.NullOrWhiteSpace(sourceSchedule);
    }

    /// <summary>
    /// Gets or sets the output schedule file path
    /// </summary>
    [Option('o', Required = true, HelpText = "The output schedule file path")]
    public string OutputSchedule
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

    /// <summary>
    /// Gets or sets the source schedule file path
    /// </summary>
    [Option('i', Required = true, HelpText = "The source schedule file path")]
    public string SourceSchedule
    {
        get;
    }
}
