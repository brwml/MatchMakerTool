using CommandLine;

namespace MatchMaker.Tool
{
    [Verb("reporting", HelpText = "Generate a report from the results XML files")]
    internal class ReportingOptions : BaseOptions
    {
        public const string DefaultRankingProcedure = "whse";

        [Option('m', Default = 0, HelpText = "The number of alternate tournament teams to create.")]
        public int NumberOfAlternateTeams
        {
            get; set;
        }

        [Option('t', Default = 0, HelpText = "The number of teams in the tournament.")]
        public int NumberOfTournamentTeams
        {
            get; set;
        }

        [Option('o', Default = ".", HelpText = "Output folder for the report.")]
        public string OutputFolder
        {
            get; set;
        }

        [Option('f', Default = OutputFormat.All, HelpText = "Output format for the report. Possible values are Excel, Html, and Pdf.")]
        public OutputFormat OutputFormat
        {
            get; set;
        }

        [Option('r', Default = DefaultRankingProcedure, HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors.")]
        public string RankingProcedure
        {
            get; set;
        }

        [Option('s', Required = true, HelpText = "Source folder with the score files.")]
        public string SourceFolder
        {
            get; set;
        }
    }
}