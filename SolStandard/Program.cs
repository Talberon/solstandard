using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using SolStandard.Utility.System;

namespace SolStandard
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ConfigureLogger();

            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            using GameDriver game = new GameDriver();
            game.Run();
        }

        private static void ConfigureLogger()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // Targets where to log to: File and Console
            FileTarget logfile = new FileTarget("logfile")
            {
                FileName = Path.Combine(Path.GetTempPath(), WindowsFileIO.GameFolder, "logs.txt")
            };

            ConsoleTarget logconsole = new ConsoleTarget("logconsole");
           
            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            // Apply config
            LogManager.Configuration = config;
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Logger.Error("MyHandler caught : " + e.Message);
            Logger.Error("Runtime terminating: " + args.IsTerminating);
            Logger.Error(e);
        }
    }
#endif
}