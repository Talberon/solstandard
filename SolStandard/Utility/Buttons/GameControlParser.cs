using System;
using System.Collections.Generic;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons
{
    public class GameControlParser : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlParser(IController controller)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.Up, controller.CursorUp},
                {Input.Down, controller.CursorDown},
                {Input.Left, controller.CursorLeft},
                {Input.Right, controller.CursorRight},

                {Input.CameraUp, controller.CameraUp},
                {Input.CameraDown, controller.CameraDown},
                {Input.CameraLeft, controller.CameraLeft},
                {Input.CameraRight, controller.CameraRight},

                {Input.Menu, controller.Menu},
                {Input.Status, controller.Status},

                {Input.Confirm, controller.Confirm},
                {Input.Cancel, controller.Cancel},
                {Input.ResetToUnit, controller.ResetToUnit},
                {Input.CenterCamera, controller.CenterCamera},

                {Input.LeftBumper, controller.SetWideZoom},
                {Input.RightBumper, controller.SetCloseZoom},
                {Input.LeftTrigger, controller.AdjustZoomOut},
                {Input.RightTrigger, controller.AdjustZoomIn},
            };
        }

        public override bool Press(Input input, PressType pressType)
        {
            switch (pressType)
            {
                case PressType.DelayedRepeat:
                    return DelayedRepeat(buttonMap[input]);
                case PressType.InstantRepeat:
                    return InstantRepeat(buttonMap[input]);
                case PressType.Single:
                    return SinglePress(buttonMap[input]);
                default:
                    throw new ArgumentOutOfRangeException("pressType", pressType, null);
            }
        }

        public override bool Released(Input input)
        {
            return buttonMap[input].Released;
        }
    }
}