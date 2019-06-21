using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.KeyboardInput
{
    public class KeyboardController : IController
    {
        private readonly Dictionary<Input, InputKey> inputs;

        private static readonly Dictionary<Input, KeyboardIcon> Icons = new Dictionary<Input, KeyboardIcon>
        {
            {Input.Confirm, KeyboardIcon.Space},
            {Input.Cancel, KeyboardIcon.LeftShift},
            {Input.PreviewUnit, KeyboardIcon.Q},
            {Input.PreviewItem, KeyboardIcon.E},

            {Input.CursorUp, KeyboardIcon.W},
            {Input.CursorDown, KeyboardIcon.S},
            {Input.CursorLeft, KeyboardIcon.A},
            {Input.CursorRight, KeyboardIcon.D},

            {Input.CameraUp, KeyboardIcon.Up},
            {Input.CameraDown, KeyboardIcon.Down},
            {Input.CameraLeft, KeyboardIcon.Left},
            {Input.CameraRight, KeyboardIcon.Right},

            {Input.Menu, KeyboardIcon.Enter},
            {Input.Status, KeyboardIcon.Escape},

            {Input.TabLeft, KeyboardIcon.Tab},
            {Input.TabRight, KeyboardIcon.R},
            {Input.ZoomOut, KeyboardIcon.LeftCtrl},
            {Input.ZoomIn, KeyboardIcon.LeftAlt}
        };

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

            inputs = new Dictionary<Input, InputKey>
            {
                {Input.Confirm, (InputKey) Confirm},
                {Input.Cancel, (InputKey) Cancel},
                {Input.PreviewUnit, (InputKey) ResetToUnit},
                {Input.PreviewItem, (InputKey) CenterCamera},

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

                {Input.TabLeft, (InputKey) SetWideZoom},
                {Input.TabRight, (InputKey) SetCloseZoom},
                {Input.ZoomOut, (InputKey) AdjustZoomOut},
                {Input.ZoomIn, (InputKey) AdjustZoomIn}
            };
        }

        public GameControl GetInput(Input input)
        {
            return inputs[input];
        }

        public ControlType ControlType => ControlType.Keyboard;

        public IRenderable GetInputIcon(Input input, Vector2 iconSize)
        {
            if (input == Input.None) return new RenderBlank();

            return KeyboardIconProvider.GetKeyboardIcon(Icons[input], iconSize);
        }

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