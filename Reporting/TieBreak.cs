﻿using System.Text.RegularExpressions;

namespace MatchMaker.Reporting
{
    public class TieBreak
    {
        public static TieBreak None = new NullTieBreak();
        public TieBreakReason Reason { get; set; }

        public override string ToString()
        {
            return Regex.Replace(this.Reason.ToString(), "[A-Z]", m => " " + m.Value).Trim();
        }

        private class NullTieBreak : TieBreak
        {
            public override string ToString()
            {
                return string.Empty;
            }
        }
    }
}