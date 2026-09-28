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
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="type"/> is not a recognized <see cref="TournamentType"/>.</exception>
    public static ITournamentRoundGenerator For(TournamentType type)
    {
        return type switch
        {
            TournamentType.RoundRobin => new RoundRobinRoundGenerator(),
            TournamentType.SingleElimination => new SingleEliminationRoundGenerator(),
            TournamentType.DoubleElimination => new DoubleEliminationRoundGenerator(),
            TournamentType.TripleElimination => new TripleEliminationRoundGenerator(),
            TournamentType.Swiss => new SwissRoundGenerator(),
            TournamentType.SwissWithTopCut => new SwissWithTopCutRoundGenerator(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown tournament type."),
        };
    }
}
