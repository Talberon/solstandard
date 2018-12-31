using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons
{
    public interface IController
    {
        
        
        GameControl Confirm { get; }
        GameControl Cancel { get; }
        GameControl ResetToUnit { get; }
        GameControl CenterCamera { get; }

        GameControl CursorUp { get; }
        GameControl CursorDown { get; }
        GameControl CursorLeft { get; }
        GameControl CursorRight { get; }

        GameControl CameraUp { get; }
        GameControl CameraDown { get; }
        GameControl CameraLeft { get; }
        GameControl CameraRight { get; }

        GameControl Menu { get; }
        GameControl Status { get; }

        GameControl SetWideZoom { get; }
        GameControl SetCloseZoom { get; }
        GameControl AdjustZoomOut { get; }
        GameControl AdjustZoomIn { get; }
    }
}