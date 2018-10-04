using CommandLine;

namespace MatchMaker.Tool
{
    internal class BaseOptions
    {
        [Option('v', HelpText = "Display verbose output", Default = false)]
        public bool Verbose
        {
            get; set;
        }
    }
}