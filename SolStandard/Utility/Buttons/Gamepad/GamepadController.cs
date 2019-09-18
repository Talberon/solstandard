using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public class GamepadController : IController
    {
        private readonly Dictionary<Input, GameControl> inputs;

        private readonly Dictionary<Input, ButtonIcon> icons = new Dictionary<Input, ButtonIcon>
        {
            {Input.Confirm, ButtonIcon.A},
            {Input.Cancel, ButtonIcon.B},
            {Input.PreviewUnit, ButtonIcon.X},
            {Input.PreviewItem, ButtonIcon.Y},

            {Input.CursorUp, ButtonIcon.DpadUp},
            {Input.CursorDown, ButtonIcon.DpadDown},
            {Input.CursorLeft, ButtonIcon.DpadLeft},
            {Input.CursorRight, ButtonIcon.DpadRight},

            {Input.CameraUp, ButtonIcon.RightStick},
            {Input.CameraDown, ButtonIcon.RightStick},
            {Input.CameraLeft, ButtonIcon.RightStick},
            {Input.CameraRight, ButtonIcon.RightStick},

            {Input.Menu, ButtonIcon.Menu},
            {Input.Status, ButtonIcon.Windows},

            {Input.TabLeft, ButtonIcon.Lb},
            {Input.TabRight, ButtonIcon.Rb},
            {Input.ZoomOut, ButtonIcon.Lt},
            {Input.ZoomIn, ButtonIcon.Rt}
        };

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

        public ControlType ControlType => ControlType.Gamepad;

        public IRenderable GetInputIcon(Input input, Vector2 iconSize)
        {
            if (input == Input.None) return RenderBlank.Blank;

            return ButtonIconProvider.GetButton(icons[input], iconSize);
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