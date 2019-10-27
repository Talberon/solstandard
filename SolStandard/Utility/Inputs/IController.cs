using System.Collections.Generic;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs
{
    public interface IController
    {
        ControlType ControlType { get; }
        GameControl GetInput(Input input);
        void RemapControl(Input inputToRemap, GameControl newInput);
        
        Dictionary<Input, GameControl> Inputs { get; }

        GameControl Confirm { get; }
        GameControl Cancel { get; }
        GameControl PreviewUnit { get; }
        GameControl PreviewItem { get; }

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