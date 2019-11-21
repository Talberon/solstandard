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

            using GameDriver game = new GameDriver();
            game.Run();
        }

        private static void ConfigureLogger()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // Targets where to log to: File and Console
            FileTarget logFile = new FileTarget("logfile")
            {
                FileName = Path.Combine(Path.GetTempPath(), WindowsFileIO.GameFolder, "logs.txt")
            };

            ConsoleTarget logConsole = new ConsoleTarget("logconsole");
            DebuggerTarget logDebugger = new DebuggerTarget();

            // Rules for mapping loggers to targets
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logDebugger);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

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
}