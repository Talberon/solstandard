using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using SolStandard.Utility.System;

namespace SolStandard
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        private static void Main()
        {
            ConfigureLogger();

            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;

            using var game = new GameDriver();
            game.Run();
        }

        private static void ConfigureLogger()
        {
            var config = new LoggingConfiguration();

            // Targets where to log to: File and Console
            var logFile = new FileTarget("logfile")
            {
                FileName = Path.Combine(Path.GetTempPath(), TemporaryFilesIO.GameFolder, "logs.txt")
            };

            var logConsole = new ConsoleTarget("logconsole");
            var logDebugger = new DebuggerTarget();

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logDebugger);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

            // Apply config
            LogManager.Configuration = config;
        }

        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception) args.ExceptionObject;
            Logger.Error("MyHandler caught : " + e.Message);
            Logger.Error("Runtime terminating: " + args.IsTerminating);
            Logger.Error(e);
        }
    }
}