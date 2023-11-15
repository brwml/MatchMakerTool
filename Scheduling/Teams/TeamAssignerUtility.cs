namespace MatchMaker.Scheduling.Teams;

using System;

internal static class TeamAssignerUtility
{
    public static int GetNumberOfTeams(int numberOfRooms, int numberOfTeams)
    {
        if (numberOfTeams != 0)
        {
            return numberOfTeams;
        }

        return numberOfRooms * 2;
    }

    public static int GetMaxTeamSize(int numberOfTeams, int numberOfQuizzers)
    {
        var (Quotient, Remainder) = Math.DivRem(numberOfQuizzers, numberOfTeams);
        return Quotient + (Remainder > 0 ? 1 : 0);
    }
}
