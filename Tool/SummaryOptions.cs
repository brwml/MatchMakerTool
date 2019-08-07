namespace MatchMaker.Tool
{
    using CommandLine;

    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SummaryOptions" />
    /// </summary>
    [Verb("summary", HelpText = "Generate a summary report for multiple events")]
    internal class SummaryOptions : BaseOptions
    {
        /// <summary>
        /// Gets or sets the input paths
        /// </summary>
        [Option('i', Required = true, HelpText = "The list of input paths", Separator = ',')]
        public IEnumerable<string> InputPaths { get; set; }

        /// <summary>
        /// Gets or sets the output path
        /// </summary>
        [Option('o', Required = true, HelpText = "The output path")]
        public string OutputPath { get; set; }
    }
}
