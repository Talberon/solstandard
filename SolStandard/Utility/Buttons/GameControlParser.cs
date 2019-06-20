using System;
using System.Collections.Generic;

namespace SolStandard.Utility.Buttons
{
    public class GameControlParser : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlParser(IController controller) : base(controller.ControlType)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.CursorUp, controller.CursorUp},
                {Input.CursorDown, controller.CursorDown},
                {Input.CursorLeft, controller.CursorLeft},
                {Input.CursorRight, controller.CursorRight},

                {Input.CameraUp, controller.CameraUp},
                {Input.CameraDown, controller.CameraDown},
                {Input.CameraLeft, controller.CameraLeft},
                {Input.CameraRight, controller.CameraRight},

                {Input.Menu, controller.Menu},
                {Input.Status, controller.Status},

                {Input.Confirm, controller.Confirm},
                {Input.Cancel, controller.Cancel},
                {Input.PreviewUnit, controller.ResetToUnit},
                {Input.PreviewItem, controller.CenterCamera},

                {Input.TabLeft, controller.SetWideZoom},
                {Input.TabRight, controller.SetCloseZoom},
                {Input.ZoomOut, controller.AdjustZoomOut},
                {Input.ZoomIn, controller.AdjustZoomIn},
            };
        }

        public override bool Press(Input input, PressType pressType)
        {
            switch (pressType)
            {
                case PressType.DelayedRepeat:
                    return DelayedRepeat(buttonMap[input], true);
                case PressType.InstantRepeat:
                    return InstantRepeat(buttonMap[input]);
                case PressType.Single:
                    return SinglePress(buttonMap[input], true);
                default:
                    throw new ArgumentOutOfRangeException("pressType", pressType, null);
            }
        }

        public override bool Peek(Input input, PressType pressType)
        {
            switch (pressType)
            {
                case PressType.DelayedRepeat:
                    return DelayedRepeat(buttonMap[input], false);
                case PressType.InstantRepeat:
                    return InstantRepeat(buttonMap[input]);
                case PressType.Single:
                    return SinglePress(buttonMap[input], false);
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