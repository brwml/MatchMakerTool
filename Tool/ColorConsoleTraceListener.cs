namespace MatchMaker.Tool;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// Defines the <see cref="ColorConsoleTraceListener" />
/// </summary>
public class ColorConsoleTraceListener : TraceListener
{
    /// <summary>
    /// Defines the event color
    /// </summary>
    private readonly Dictionary<TraceEventType, ConsoleColor> eventColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorConsoleTraceListener"/> class.
    /// </summary>
    public ColorConsoleTraceListener()
    {
        this.eventColor = new Dictionary<TraceEventType, ConsoleColor>
            {
                { TraceEventType.Verbose, ConsoleColor.DarkGray },
                { TraceEventType.Information, ConsoleColor.Gray },
                { TraceEventType.Warning, ConsoleColor.Yellow },
                { TraceEventType.Error, ConsoleColor.DarkRed },
                { TraceEventType.Critical, ConsoleColor.Red },
                { TraceEventType.Start, ConsoleColor.DarkCyan },
                { TraceEventType.Stop, ConsoleColor.DarkCyan }
            };
    }

    /// <inheritdoc />
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
        Console.ForegroundColor = this.GetEventColor(eventType);
        this.WriteLine(message);
        Console.ResetColor();
    }

    /// <inheritdoc />
    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
        this.TraceEvent(eventCache, source, eventType, id, string.Format(CultureInfo.InvariantCulture, format, args));
    }

    /// <inheritdoc />
    public override void Write(string message)
    {
        Console.Write(message);
    }

    /// <inheritdoc />
    public override void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    /// Gets the <see cref="ConsoleColor"/> corresponding to the <see cref="TraceEventType"/>
    /// </summary>
    /// <param name="eventType">The <see cref="TraceEventType"/></param>
    /// <returns>The <see cref="ConsoleColor"/></returns>
    private ConsoleColor GetEventColor(TraceEventType eventType)
    {
        return !this.eventColor.ContainsKey(eventType) ? Console.ForegroundColor : this.eventColor[eventType];
    }
}
