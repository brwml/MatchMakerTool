namespace MatchMaker.Scheduling.Tournaments;

using System;

using MatchMaker.Models;

/// <summary>
/// Creates the <see cref="ITournamentRoundGenerator"/> instance appropriate for a given
/// <see cref="TournamentType"/>, so callers that advance a tournament in progress do not need to
/// branch on the tournament type themselves.
/// </summary>
public static class TournamentRoundGeneratorFactory
{
    /// <summary>
    /// Gets the <see cref="ITournamentRoundGenerator"/> for the given <see cref="TournamentType"/>.
    /// </summary>
    /// <param name="type">The tournament type.</param>
    /// <returns>The <see cref="ITournamentRoundGenerator"/> instance.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when round generation for <paramref name="type"/> has not yet been implemented.
    /// </exception>
    public static ITournamentRoundGenerator For(TournamentType type)
    {
        return type switch
        {
            TournamentType.RoundRobin => new RoundRobinRoundGenerator(),
            TournamentType.SingleElimination => new SingleEliminationRoundGenerator(),
            TournamentType.DoubleElimination => throw new NotSupportedException("Double-elimination round generation is not yet implemented."),
            TournamentType.TripleElimination => throw new NotSupportedException("Triple-elimination round generation is not yet implemented."),
            TournamentType.Swiss => throw new NotSupportedException("Swiss round generation is not yet implemented."),
            TournamentType.SwissWithTopCut => throw new NotSupportedException("Swiss-with-top-cut round generation is not yet implemented."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tournament type."),
        };
    }
}
