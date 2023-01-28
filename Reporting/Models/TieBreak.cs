namespace MatchMaker.Reporting.Models;

using System.Runtime.Serialization;

using Humanizer;

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
    /// Initializes a new instance of the <see cref="TieBreak"/> class.
    /// </summary>
    /// <param name="reason">The tie-break reason</param>
    public TieBreak(TieBreakReason reason)
    {
        this.Reason = reason;
    }

    /// <summary>
    /// Gets or sets the Reason
    /// </summary>
    [DataMember]
    public TieBreakReason Reason
    {
        get;
    }

    /// <summary>
    /// Creates a <see cref="string"/> for the tie breaker
    /// </summary>
    /// <returns>The <see cref="string"/></returns>
    public override string ToString()
    {
        return this.Reason.Humanize(LetterCasing.Title);
    }

    /// <summary>
    /// Defines the <see cref="NullTieBreak" />
    /// </summary>
    private class NullTieBreak : TieBreak
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullTieBreak"/> class.
        /// </summary>
        public NullTieBreak()
            : base(TieBreakReason.None)
        {
        }

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
