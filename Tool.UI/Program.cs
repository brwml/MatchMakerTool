using System;

namespace MatchMaker.Tool.UI
{
    internal static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new MatchMakerToolApplication();
            app.Run(new MatchMakerToolWindow());
        }
    }
}