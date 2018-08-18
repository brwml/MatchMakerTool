using System.Collections.Generic;
using CommandLine;

namespace MatchMaker.Tool
{
    [Verb("summary", HelpText = "Generate a summary report for multiple events")]
    public class SummaryOptions
    {
        [Option('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
        public IEnumerable<string> InputPaths
        {
            get; set;
        }

        [Option('o', Required = true, HelpText = "The output path")]
        public string OutputPath
        {
            get; set;
        }
    }
}