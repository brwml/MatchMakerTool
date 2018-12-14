using System;

namespace MatchMaker.Tool
{
    [Flags]
    internal enum OutputFormat
    {
        None = 0,
        Excel = 1,
        Html = 2,
        Pdf = 4,
        All = Excel | Html | Pdf
    }
}