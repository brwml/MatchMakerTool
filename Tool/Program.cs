﻿namespace MatchMaker.Tool
{
    using CommandLine;

    using System;
    using System.Diagnostics;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Gets the option types
        /// </summary>
        private static Type[] OptionTypes => new[] { typeof(ReportingOptions), typeof(SummaryOptions), typeof(ScheduleOptions) };

        /// <summary>
        /// The main program entry point
        /// </summary>
        /// <param name="args">The program arguments</param>
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

        /// <summary>
        /// Processes the base options.
        /// </summary>
        /// <param name="options">The options<see cref="BaseOptions"/></param>
        private static void ProcessBaseOptions(BaseOptions options)
        {
            if (options.Verbose)
            {
                Trace.Listeners.Add(new ColorConsoleTraceListener());
            }
        }
    }
}
