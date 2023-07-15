namespace MatchMaker.Models;
/// <summary>
/// Defines the <see cref="TieBreakReason"/>
/// </summary>
public enum TieBreakReason
{
    /// <summary>
    /// Defines the no tie breaker
    /// </summary>
    None,

    /// <summary>
    /// Defines the head-to-head tie breaker
    /// </summary>
    HeadToHead,

    /// <summary>
    /// Defines the average score tie breaker
    /// </summary>
    AverageScore,

    /// <summary>
    /// Defines the average errors tie breaker
    /// </summary>
    AverageErrors
}
