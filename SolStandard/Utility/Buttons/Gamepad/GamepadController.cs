using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadController : IController
    {
        public GamepadController(PlayerIndex playerIndex)
        {
            Confirm = new GamepadA(playerIndex);
            Cancel = new GamepadB(playerIndex);
            ActiveUnit = new GamepadX(playerIndex);
            CenterCamera = new GamepadY(playerIndex);

            CursorUp = new GamepadUp(playerIndex);
            CursorDown = new GamepadDown(playerIndex);
            CursorLeft = new GamepadLeft(playerIndex);
            CursorRight = new GamepadRight(playerIndex);

            CameraUp = new GamepadRsUp(playerIndex);
            CameraDown = new GamepadRsDown(playerIndex);
            CameraLeft = new GamepadRsLeft(playerIndex);
            CameraRight = new GamepadRsRight(playerIndex);

            Menu = new GamepadStart(playerIndex);
            Status = new GamepadSelect(playerIndex);

            SetWideZoom = new GamepadLeftBumper(playerIndex);
            SetCloseZoom = new GamepadRightBumper(playerIndex);
            AdjustZoomOut = new GamepadLeftTrigger(playerIndex);
            AdjustZoomIn = new GamepadRightTrigger(playerIndex);
        }

        public GameControl Confirm { get; private set; }
        public GameControl Cancel { get; private set; }
        public GameControl ActiveUnit { get; private set; }
        public GameControl CenterCamera { get; private set; }

        public GameControl CursorUp { get; private set; }
        public GameControl CursorDown { get; private set; }
        public GameControl CursorLeft { get; private set; }
        public GameControl CursorRight { get; private set; }

        public GameControl CameraUp { get; private set; }
        public GameControl CameraDown { get; private set; }
        public GameControl CameraLeft { get; private set; }
        public GameControl CameraRight { get; private set; }

        public GameControl Menu { get; private set; }
        public GameControl Status { get; private set; }

        public GameControl SetWideZoom { get; private set; }
        public GameControl SetCloseZoom { get; private set; }
        public GameControl AdjustZoomOut { get; private set; }
        public GameControl AdjustZoomIn { get; private set; }
    }
}