namespace MatchMaker.Reporting.Exporters
{
    using System;

    using MatchMaker.Reporting.Models;

    /// <summary>
    /// The team information
    /// </summary>
    public class TeamInfo
    {
        /// <summary>
        /// Initializes an instance of the <see cref="TeamInfo"/> class
        /// </summary>
        /// <param name="team">The team</param>
        /// <param name="summary">The team summary</param>
        public TeamInfo(Team team, TeamSummary summary)
        {
            this.Abbreviation = team.Abbreviation;
            this.Id = team.Id;
            this.Name = team.Name;
            this.Losses = summary.Losses;
            this.Place = summary.Place;
            this.TieBreak = summary.TieBreak;
            this.TotalErrors = summary.TotalErrors;
            this.TotalRounds = summary.TotalRounds;
            this.TotalScore = summary.TotalScore;
            this.Wins = summary.Wins;
            this.ShowPlace = true;
        }

        /// <summary>
        /// Gets or sets the Abbreviation
        /// </summary>
        public string Abbreviation { get; }

        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the Losses
        /// </summary>
        public int Losses { get; }

        /// <summary>
        /// Gets or sets the Place
        /// </summary>
        public int Place { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the place.
        /// </summary>
        public bool ShowPlace { get; set; }

        /// <summary>
        /// Gets or sets the tie breaker
        /// </summary>
        public TieBreak TieBreak { get; }

        /// <summary>
        /// Gets or sets the total errors
        /// </summary>
        public int TotalErrors { get; }

        /// <summary>
        /// Gets the total rounds
        /// </summary>
        public int TotalRounds { get; }

        /// <summary>
        /// Gets or sets the total score
        /// </summary>
        public int TotalScore { get; }

        /// <summary>
        /// Gets the win percentage
        /// </summary>
        public decimal WinPercentage => Convert.ToDecimal(this.Wins) / Convert.ToDecimal(this.TotalRounds);

        /// <summary>
        /// Gets or sets the Wins
        /// </summary>
        public int Wins { get; }

        /// <summary>
        /// Gets the average errors
        /// </summary>
        public decimal AverageErrors => Convert.ToDecimal(this.TotalErrors) / Convert.ToDecimal(this.TotalRounds);

        /// <summary>
        /// Gets the average score
        /// </summary>
        public decimal AverageScore => Convert.ToDecimal(this.TotalScore) / Convert.ToDecimal(this.TotalRounds);
    }
}
