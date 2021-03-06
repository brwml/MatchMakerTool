﻿namespace MatchMaker.Reporting.Models
{
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Defines the <see cref="TieBreak" />
    /// </summary>
    [DataContract]
    public class TieBreak
    {
        /// <summary>
        /// Defines a tie breaker that does not resolve a tie.
        /// </summary>
        public static readonly TieBreak None = new NullTieBreak();

        /// <summary>
        /// Gets or sets the Reason
        /// </summary>
        [DataMember]
        public TieBreakReason Reason { get; set; }

        /// <summary>
        /// Creates a <see cref="string"/> for the tie breaker
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        public override string ToString()
        {
            return Regex.Replace(this.Reason.ToString(), "[A-Z]", m => " " + m.Value).Trim();
        }

        /// <summary>
        /// Defines the <see cref="NullTieBreak" />
        /// </summary>
        private class NullTieBreak : TieBreak
        {
            /// <summary>
            /// The ToString
            /// </summary>
            /// <returns>The <see cref="string"/></returns>
            public override string ToString()
            {
                return string.Empty;
            }
        }
    }
}
