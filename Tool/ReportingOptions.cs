using CommandLine;

namespace MatchMaker.Tool
{
    internal class ReportingOptions
    {
        [Option('m', DefaultValue = 0, HelpText = "The number of alternate tournament teams to create.")]
        public int NumberOfAlternateTeams { get; set; }

        [Option('t', DefaultValue = 0, HelpText = "The number of teams in the tournament.")]
        public int NumberOfTournamentTeams { get; set; }

        [Option('o', DefaultValue = ".", HelpText = "Output folder for the report.")]
        public string OutputFolder { get; set; }

        [Option('f', DefaultValue = OutputFormat.None, HelpText = "Output format for the report. Possible values are Excel, Html, and Pdf.")]
        public OutputFormat OutputFormat { get; set; }

        [Option('r', DefaultValue = "whse", HelpText = "The ranking operations and sequence. Each character represents a ranking operation. Possible operations include 'w' for winning percentage, 'l' for total losses, 'h' for head-to-head competition, 's' for average score, and 'e' for average errors.")]
        public string RankingProcedure { get; set; }

        [Option('s', Required = true, HelpText = "Source folder with the score files.")]
        public string SourceFolder { get; set; }
    }
}