using System.Collections.Generic;
using CommandLine;

namespace MatchMaker.Tool
{
    public class SummaryOptions
    {
        [OptionList('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
        public IEnumerable<string> InputPaths { get; set; }

        [Option('o', Required = true, HelpText = "The output path")]
        public string OutputPath { get; set; }
    }
}