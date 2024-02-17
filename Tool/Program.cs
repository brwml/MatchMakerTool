namespace MatchMaker.Tool;

using System;
using System.Diagnostics;

using CommandLine;

using MatchMaker.Tool.Controllers;

/// <summary>
/// Defines the <see cref="Program" />
/// </summary>
internal class Program
{
    /// <summary>
    /// Gets the option types
    /// </summary>
    private static Type[] OptionTypes =>
    [
        typeof(ReportingOptions),
        typeof(SummaryOptions),
        typeof(ScheduleOptions)
    ];

    /// <summary>
    /// The main program entry point
    /// </summary>
    /// <param name="args">The program arguments</param>
    private static void Main(string[] args)
    {
        try
        {
            _ = Parser.Default.ParseArguments(args, OptionTypes)
                .WithParsed<BaseOptions>(ProcessBaseOptions)
                .MapResult(
                    (ReportingOptions options) => new ReportingController().Process(options),
                    (SummaryOptions options) => new SummaryController().Process(options),
                    (ScheduleOptions options) => new SchedulingController().Process(options),
                    errors => false);
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
        }
    }

    /// <summary>
    /// Processes the base options.
    /// </summary>
    /// <param name="options">The options<see cref="BaseOptions"/></param>
    private static void ProcessBaseOptions(BaseOptions options)
    {
        if (options.Verbose)
        {
            _ = Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.IndentSize = 2;
        }
    }
}
