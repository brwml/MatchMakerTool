using CommandLine;
using CommandLine.Text;

namespace MatchMaker.Tool
{
    internal class Options
    {
        public const string ReportingOption = "report";

        [VerbOption(ReportingOption, HelpText = "Generate a report from the results XML files")]
        public ReportingOptions ReportingOptions { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}