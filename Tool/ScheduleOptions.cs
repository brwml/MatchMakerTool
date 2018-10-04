using CommandLine;

namespace MatchMaker.Tool
{
    [Verb("schedule", HelpText = "Generates a tournament schedule")]
    internal class ScheduleOptions : BaseOptions
    {
        [Option('a', Default = true, Required = false, HelpText = "Indicates whether the schedule is created a priori", SetName = "Swiss")]
        public bool IsApriori
        {
            get; set;
        }

        [Option('s', Default = true, Required = false, HelpText = "Indicates whether the tournament is seeded")]
        public bool IsSeeded
        {
            get; set;
        }

        [Option('o', Required = true, HelpText = "The output schedule file path")]
        public string OutputSchedule
        {
            get; set;
        }

        [Option('r', Required = false, HelpText = "The results folder", SetName = "Swiss")]
        public string ResultsFolder
        {
            get; set;
        }

        [Option('n', Required = false, HelpText = "The number of rooms available")]
        public int Rooms
        {
            get; set;
        }

        [Option('t', Default = ScheduleType.RoundRobin, Required = false, HelpText = "The schedule type. Options include RoundRobin and Swiss.")]
        public ScheduleType ScheduleType
        {
            get; set;
        }

        [Option('i', Required = true, HelpText = "The source schedule file path")]
        public string SourceSchedule
        {
            get; set;
        }
    }
}