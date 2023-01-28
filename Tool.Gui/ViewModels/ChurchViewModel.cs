namespace MatchMaker.Tool.Gui.ViewModels;

/// <summary>
/// The church view model
/// </summary>
internal class ChurchViewModel : IEquatable<ChurchViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChurchViewModel"/> class.
    /// </summary>
    /// <param name="name">The name of the church</param>
    public ChurchViewModel(string name)
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets the name of the church.
    /// </summary>
    public string Name
    {
        get; set;
    }

    /// <summary>
    /// Determines whether the other view model instance is equal to the current view model instance.
    /// </summary>
    /// <param name="other">The other view model instance</param>
    /// <returns><c>true</c> if the other view model instance is equal to the current; otherwise <c>false</c></returns>
    public bool Equals(ChurchViewModel? other)
    {
        return other?.Name?.Equals(this.Name, StringComparison.OrdinalIgnoreCase) ?? false;
    }

    /// <summary>
    /// Converts the church instance to a string.
    /// </summary>
    /// <returns>The string representation of the church</returns>
    public override string ToString()
    {
        return this.Name;
    }

    /// <summary>
    /// Determines whether the other object instance is equal to the current object instance.
    /// </summary>
    /// <param name="other">The other object instance</param>
    /// <returns><c>true</c> if the other object instance is equal to the current; otherwise <c>false</c></returns>
    public override bool Equals(object? other)
    {
        return this.Equals(other as ChurchViewModel);
    }

    /// <summary>
    /// Gets the hash code for the current instance.
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode()
    {
        return this.Name.GetHashCode();
    }
}
