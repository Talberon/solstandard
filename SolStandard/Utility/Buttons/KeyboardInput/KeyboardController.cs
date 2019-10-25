using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class KeyboardController : IController
    {
        private readonly Dictionary<Input, GameControl> inputs;

        public KeyboardController()
        {
            Confirm = new InputKey(Keys.Space);
            Cancel = new InputKey(Keys.LeftShift);
            ResetToUnit = new InputKey(Keys.Q);
            CenterCamera = new InputKey(Keys.E);

            CursorUp = new InputKey(Keys.W);
            CursorDown = new InputKey(Keys.S);
            CursorLeft = new InputKey(Keys.A);
            CursorRight = new InputKey(Keys.D);

            CameraUp = new InputKey(Keys.Up);
            CameraDown = new InputKey(Keys.Down);
            CameraLeft = new InputKey(Keys.Left);
            CameraRight = new InputKey(Keys.Right);

            Menu = new InputKey(Keys.Enter);
            Status = new InputKey(Keys.Escape);

            SetWideZoom = new InputKey(Keys.Tab);
            SetCloseZoom = new InputKey(Keys.R);
            AdjustZoomOut = new InputKey(Keys.LeftControl);
            AdjustZoomIn = new InputKey(Keys.LeftAlt);

            inputs = new Dictionary<Input, GameControl>
            {
                {Input.None, new VoidInput()},

                {Input.Confirm, Confirm},
                {Input.Cancel, Cancel},
                {Input.PreviewUnit, ResetToUnit},
                {Input.PreviewItem, CenterCamera},

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

                {Input.TabLeft, SetWideZoom},
                {Input.TabRight, SetCloseZoom},
                {Input.ZoomOut, AdjustZoomOut},
                {Input.ZoomIn, AdjustZoomIn}
            };
        }

        public GameControl GetInput(Input input)
        {
            return inputs[input];
        }

        public ControlType ControlType => ControlType.Keyboard;

        public GameControl Confirm { get; }
        public GameControl Cancel { get; }
        public GameControl ResetToUnit { get; }
        public GameControl CenterCamera { get; }

        public GameControl CursorUp { get; }
        public GameControl CursorDown { get; }
        public GameControl CursorLeft { get; }
        public GameControl CursorRight { get; }

        public GameControl CameraUp { get; }
        public GameControl CameraDown { get; }
        public GameControl CameraLeft { get; }
        public GameControl CameraRight { get; }

        public GameControl Menu { get; }
        public GameControl Status { get; }

        public GameControl SetWideZoom { get; }
        public GameControl SetCloseZoom { get; }
        public GameControl AdjustZoomOut { get; }
        public GameControl AdjustZoomIn { get; }
    }
}