using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace MatchMaker.Tool
{
    public class ColorConsoleTraceListener : TraceListener
    {
        private readonly Dictionary<TraceEventType, ConsoleColor> eventColor;

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

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            Console.ForegroundColor = this.GetEventColor(eventType);
            this.WriteLine(message);
            Console.ResetColor();
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.TraceEvent(eventCache, source, eventType, id, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public override void Write(string message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        private ConsoleColor GetEventColor(TraceEventType eventType)
        {
            return !this.eventColor.ContainsKey(eventType) ? Console.ForegroundColor : this.eventColor[eventType];
        }
    }
}