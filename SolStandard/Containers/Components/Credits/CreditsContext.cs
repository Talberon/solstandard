using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map.Elements;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Components.Credits
{
    public class CreditsContext 
    {
        public readonly ScrollingTextPaneHUD CreditsHUD;
        private GlobalContext.GameState previousGameState;
        private const string CreditsPath = "/credits";

        public CreditsContext(ScrollingTextPaneHUD creditsHUD)
        {
            CreditsHUD = creditsHUD;
        }

        public void OpenView()
        {
            if (GlobalContext.CurrentGameState == GlobalContext.GameState.Credits) return;

            previousGameState = GlobalContext.CurrentGameState;
            GlobalContext.CurrentGameState = GlobalContext.GameState.Credits;
        }

        public void ExitView()
        {
            AssetManager.MapUnitCancelSFX.Play();
            GlobalContext.CurrentGameState = previousGameState;
        }

        public void ScrollWindow(Direction direction)
        {
            CreditsHUD.ScrollContents(direction);
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
        }public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}