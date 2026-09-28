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
    /// <returns>0 when the requested operation succeeds; otherwise a non-zero exit code.</returns>
    private static int Main(string[] args)
    {
        try
        {
            var succeeded = Parser.Default.ParseArguments(args, OptionTypes)
                .WithParsed<BaseOptions>(ProcessBaseOptions)
                .MapResult(
                    (ReportingOptions options) => new ReportingController().Process(options),
                    (SummaryOptions options) => new SummaryController().Process(options),
                    (ScheduleOptions options) => new SchedulingController().Process(options),
                    errors => false);

            return succeeded ? 0 : 1;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.ToString());
            Console.Error.WriteLine(e.Message);
            return 1;
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
            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.IndentSize = 2;
        }
    }
}
