using System;

namespace SolStandard
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            using (var game = new GameDriver())
                game.Run();
        }
    }
#endif
}
