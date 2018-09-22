using System;
using System.Collections.Generic;
using System.Text;

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