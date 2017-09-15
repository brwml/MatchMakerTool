using System;
using CommandLine;

namespace MatchMaker.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments(args, new Options(), ProcessVerbCommand);
        }

        private static void ProcessVerbCommand(string verb, object options)
        {
            if (verb.Equals(Options.ReportingOption, StringComparison.OrdinalIgnoreCase))
            {
                Reporting.Process((ReportingOptions)options);
            }
        }
    }
}