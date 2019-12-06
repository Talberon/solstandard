using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SolStandard.Containers.View;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Contexts
{
    public class CreditsContext
    {
        public readonly ScrollingTextPaneView CreditsView;
        private GameContext.GameState previousGameState;
        private const string CreditsPath = "/credits";

        public CreditsContext(ScrollingTextPaneView creditsView)
        {
            CreditsView = creditsView;
        }

        public void OpenView()
        {
            if (GameContext.CurrentGameState == GameContext.GameState.Credits) return;

            previousGameState = GameContext.CurrentGameState;
            GameContext.CurrentGameState = GameContext.GameState.Credits;
        }

        public void ExitView()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GameContext.CurrentGameState = previousGameState;
        }

        public void ScrollWindow(Direction direction)
        {
            CreditsView.ScrollContents(direction);
        }

        public void OpenBrowser()
        {
            AssetManager.MenuConfirmSFX.Play();
            OpenBrowser(GameDriver.SolStandardUrl + CreditsPath);
        }

        public static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw new PlatformNotSupportedException(
                    "This operating system is not supported. Use Windows/Linux/OSX to use this feature."
                );
            }
        }
    }
}