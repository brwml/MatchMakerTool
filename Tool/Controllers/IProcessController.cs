namespace MatchMaker.Tool.Controllers;

/// <summary>
/// Defines the process controller interface.
/// </summary>
/// <typeparam name="T">The options type</typeparam>
internal interface IProcessController<T> where T : BaseOptions
{
    /// <summary>
    /// Processes the specified options.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>
    /// <c>true</c> when processing is successful; otherwise <c>false</c>.
    /// </returns>
    bool Process(T options);
}
