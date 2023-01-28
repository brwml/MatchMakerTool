namespace MatchMaker.Reporting.Models;

using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml.Linq;

using Ardalis.GuardClauses;

/// <summary>
/// Defines the <see cref="QuizzerResult" />
/// </summary>
[DataContract]
[DebuggerDisplay("Quizzer Result (Quizzer {QuizzerId}, Score {Score}, Errors {Errors})")]
public class QuizzerResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuizzerResult"/> class.
    /// </summary>
    /// <param name="id">The quizzer identifier</param>
    /// <param name="score">The quizzer score</param>
    /// <param name="errors">The quizzer errors</param>
    public QuizzerResult(int id, int score, int errors)
    {
        this.QuizzerId = id;
        this.Score = score;
        this.Errors = errors;
    }

    /// <summary>
    /// Gets or sets the Errors
    /// </summary>
    [DataMember]
    public int Errors
    {
        get;
    }

    /// <summary>
    /// Gets or sets the quizzer identifier
    /// </summary>
    [DataMember]
    public int QuizzerId
    {
        get;
    }

    /// <summary>
    /// Gets or sets the Score
    /// </summary>
    [DataMember]
    public int Score
    {
        get;
    }

    /// <summary>
    /// Creates a <see cref="QuizzerResult"/> instance from an XML element.
    /// </summary>
    /// <param name="xml">The xml<see cref="XElement"/> instance</param>
    /// <returns>The <see cref="QuizzerResult"/> instance</returns>
    public static QuizzerResult FromXml(XElement xml)
    {
        Guard.Against.Null(xml);

        return new QuizzerResult(
            xml.GetAttribute<int>("id"),
            xml.GetAttribute<int>("score"),
            xml.GetAttribute<int>("errors"));
    }

    /// <summary>
    /// Converts the <see cref="QuizzerResult"/> instance to XML.
    /// </summary>
    /// <returns>The <see cref="XElement"/> instance</returns>
    public XElement ToXml()
    {
        return new XElement(
            "quizzer",
            new XAttribute("id", this.QuizzerId),
            new XAttribute("score", this.Score),
            new XAttribute("errors", this.Errors));
    }
}
