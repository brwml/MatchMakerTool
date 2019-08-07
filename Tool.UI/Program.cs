namespace MatchMaker.Tool.UI
{
    using System;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var app = new MatchMakerToolApplication();
            app.Run(new MatchMakerToolWindow());
        }
    }
}
