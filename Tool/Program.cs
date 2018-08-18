﻿using System;
using CommandLine;

namespace MatchMaker.Tool
{
    internal class Program
    {
        private static Type[] OptionTypes => new[] { typeof(ReportingOptions), typeof(SummaryOptions), typeof(ScheduleOptions) };

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments(args, OptionTypes).MapResult(
                (ReportingOptions options) => Reporting.Process(options),
                (SummaryOptions options) => Reporting.Process(options),
                (ScheduleOptions options) => Scheduling.Process(options),
                errors => false);
        }
    }
}