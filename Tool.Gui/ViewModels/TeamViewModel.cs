namespace MatchMaker.Tool.Gui.ViewModels;

/// <summary>
/// The team view model
/// </summary>
internal class TeamViewModel
{
    /// <summary>
    /// Initializes an instance of the <see cref="TeamViewModel"/> class.
    /// </summary>
    /// <param name="name">The name of the team</param>
    /// <param name="abbreviation">The abbreviation of the team</param>
    public TeamViewModel(string name, string abbreviation)
    {
        this.Name = name;
        this.Abbreviation = abbreviation;
    }

    /// <summary>
    /// Gets or sets the name of the team.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the abbreviation of the team.
    /// </summary>
    public string Abbreviation { get; set; }

    /// <summary>
    /// Gets this list of quizzers.
    /// </summary>
    public IList<QuizzerViewModel> Quizzers { get; } = new List<QuizzerViewModel>();

    /// <summary>
    /// Creates the string representation of the team view model.
    /// </summary>
    /// <returns>The string representation</returns>
    public override string ToString()
    {
        return FormattableString.Invariant($"{this.Name} ({this.Abbreviation})");
    }
}
