using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SolStandard.Utility.Network
{
    public static class Browser
    {
        public static void OpenUrl(string url)
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