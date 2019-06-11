using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons
{
    public interface IController
    {
        ControlType ControlType { get; }
        GameControl GetInput(Input input);
        IRenderable GetInputIcon(Input input, Vector2 iconSize);
        
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