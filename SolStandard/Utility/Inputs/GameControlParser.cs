using System;
using System.Collections.Generic;

namespace SolStandard.Utility.Inputs
{
    public class GameControlParser : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlParser(IController controller) : base(controller)
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
                {Input.PreviewUnit, controller.PreviewUnit},
                {Input.PreviewItem, controller.PreviewItem},

                {Input.TabLeft, controller.SetWideZoom},
                {Input.TabRight, controller.SetCloseZoom},
                {Input.ZoomOut, controller.AdjustZoomOut},
                {Input.ZoomIn, controller.AdjustZoomIn}
            };
        }

        public override bool Press(Input input, PressType pressType)
        {
            return pressType switch
            {
                PressType.DelayedRepeat => DelayedRepeat(buttonMap[input], true),
                PressType.InstantRepeat => InstantRepeat(buttonMap[input]),
                PressType.Single => SinglePress(buttonMap[input], true),
                _ => throw new ArgumentOutOfRangeException(nameof(pressType), pressType, null)
            };
        }

        public override bool Peek(Input input, PressType pressType)
        {
            return pressType switch
            {
                PressType.DelayedRepeat => DelayedRepeat(buttonMap[input], false),
                PressType.InstantRepeat => InstantRepeat(buttonMap[input]),
                PressType.Single => SinglePress(buttonMap[input], false),
                _ => throw new ArgumentOutOfRangeException(nameof(pressType), pressType, null)
            };
        }


        public override bool Released(Input input)
        {
            return buttonMap[input].Released;
        }
    }
}