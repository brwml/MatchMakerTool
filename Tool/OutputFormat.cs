using System;

namespace MatchMaker.Tool
{
    [Flags]
    internal enum OutputFormat
    {
        None,
        Excel,
        Html,
        Pdf,
        Gum
    }
}