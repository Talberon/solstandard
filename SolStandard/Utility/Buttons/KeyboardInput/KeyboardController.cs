using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class KeyboardController : IController
    {
        private readonly Dictionary<Input, InputKey> inputs;
        public KeyboardController(PlayerIndex playerIndex)
        {
            Confirm = new InputKey(playerIndex, Keys.Space);
            Cancel = new InputKey(playerIndex, Keys.LeftShift);
            ResetToUnit = new InputKey(playerIndex, Keys.Tab);
            CenterCamera = new InputKey(playerIndex, Keys.OemTilde);

            CursorUp = new InputKey(playerIndex, Keys.W);
            CursorDown = new InputKey(playerIndex, Keys.S);
            CursorLeft = new InputKey(playerIndex, Keys.A);
            CursorRight = new InputKey(playerIndex, Keys.D);

            CameraUp = new InputKey(playerIndex, Keys.Up);
            CameraDown = new InputKey(playerIndex, Keys.Down);
            CameraLeft = new InputKey(playerIndex, Keys.Left);
            CameraRight = new InputKey(playerIndex, Keys.Right);

            Menu = new InputKey(playerIndex, Keys.Enter);
            Status = new InputKey(playerIndex, Keys.Escape);

            SetWideZoom = new InputKey(playerIndex, Keys.LeftControl);
            SetCloseZoom = new InputKey(playerIndex, Keys.LeftAlt);
            AdjustZoomOut = new InputKey(playerIndex, Keys.Q);
            AdjustZoomIn = new InputKey(playerIndex, Keys.E);
            
            inputs = new Dictionary<Input, InputKey>
            {
                {Input.Confirm, (InputKey) Confirm},
                {Input.Cancel, (InputKey) Cancel},
                {Input.ResetToUnit, (InputKey) ResetToUnit},
                {Input.CenterCamera, (InputKey) CenterCamera},

                {Input.CursorUp, (InputKey) CursorUp},
                {Input.CursorDown, (InputKey) CursorDown},
                {Input.CursorLeft, (InputKey) CursorLeft},
                {Input.CursorRight, (InputKey) CursorRight},

                {Input.CameraUp, (InputKey) CameraUp},
                {Input.CameraDown, (InputKey) CameraDown},
                {Input.CameraLeft, (InputKey) CameraLeft},
                {Input.CameraRight, (InputKey) CameraRight},

                {Input.Menu, (InputKey) Menu},
                {Input.Status, (InputKey) Status},

                {Input.LeftBumper, (InputKey) SetWideZoom},
                {Input.RightBumper, (InputKey) SetCloseZoom},
                {Input.LeftTrigger, (InputKey) AdjustZoomOut},
                {Input.RightTrigger, (InputKey) AdjustZoomIn},
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