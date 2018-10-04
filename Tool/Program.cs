using System;
using System.Diagnostics;

using CommandLine;

namespace MatchMaker.Tool
{
    internal class Program
    {
        private static Type[] OptionTypes => new[] { typeof(ReportingOptions), typeof(SummaryOptions), typeof(ScheduleOptions) };

        private static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments(args, OptionTypes)
                    .WithParsed<BaseOptions>(ProcessBaseOptions)
                    .MapResult(
                        (ReportingOptions options) => Reporting.Process(options),
                        (SummaryOptions options) => Reporting.Process(options),
                        (ScheduleOptions options) => Scheduling.Process(options),
                        errors => false);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        private static void ProcessBaseOptions(BaseOptions options)
        {
            if (options.Verbose)
            {
                Trace.Listeners.Add(new ColorConsoleTraceListener());
            }
        }
    }
}