using CommandLine;

namespace MatchMaker.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ReportingOptions, SummaryOptions>(args).MapResult(
                (ReportingOptions options) => Reporting.Process(options),
                (SummaryOptions options) => Reporting.Process(options),
                error => false);
        }
    }
}