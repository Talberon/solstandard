using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadController : IController
    {
        private readonly Dictionary<Input, GameControl> inputs;

        public GamepadController(PlayerIndex playerIndex)
        {
            Confirm = new GamepadA(playerIndex);
            Cancel = new GamepadB(playerIndex);
            ResetToUnit = new GamepadX(playerIndex);
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

            inputs = new Dictionary<Input, GameControl>
            {
                {Input.Confirm, Confirm},
                {Input.Cancel, Cancel},
                {Input.ResetToUnit, ResetToUnit},
                {Input.CenterCamera, CenterCamera},

                {Input.CursorUp, CursorUp},
                {Input.CursorDown, CursorDown},
                {Input.CursorLeft, CursorLeft},
                {Input.CursorRight, CursorRight},

                {Input.CameraUp, CameraUp},
                {Input.CameraDown, CameraDown},
                {Input.CameraLeft, CameraLeft},
                {Input.CameraRight, CameraRight},

                {Input.Menu, Menu},
                {Input.Status, Status},

                {Input.LeftBumper, SetWideZoom},
                {Input.RightBumper, SetCloseZoom},
                {Input.LeftTrigger, AdjustZoomOut},
                {Input.RightTrigger, AdjustZoomIn},
            };
        }

        public GameControl GetInput(Input input)
        {
            return inputs[input];
        }

        public GameControl Confirm { get; private set; }
        public GameControl Cancel { get; private set; }
        public GameControl ResetToUnit { get; private set; }
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