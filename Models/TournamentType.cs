namespace MatchMaker.Models;

/// <summary>
/// Defines the <see cref="TournamentType"/>
/// </summary>
public enum TournamentType
{
    /// <summary>
    /// Defines a round-robin tournament, where every team plays every other team.
    /// </summary>
    RoundRobin,

    /// <summary>
    /// Defines a single-elimination tournament, where a team is eliminated after one loss.
    /// </summary>
    SingleElimination,

    /// <summary>
    /// Defines a double-elimination tournament, where a team is eliminated after two losses.
    /// </summary>
    DoubleElimination,

    /// <summary>
    /// Defines a triple-elimination tournament, where a team is eliminated after three losses.
    /// </summary>
    TripleElimination,

    /// <summary>
    /// Defines a Swiss-system tournament, where pairings for each round are recomputed from the
    /// current standings rather than fixed in advance.
    /// </summary>
    Swiss,

    /// <summary>
    /// Defines a Swiss-system tournament whose top-placing teams, after a fixed number of Swiss
    /// rounds, advance into a single- or double-elimination "top cut" bracket to determine the
    /// final standings.
    /// </summary>
    SwissWithTopCut
}
