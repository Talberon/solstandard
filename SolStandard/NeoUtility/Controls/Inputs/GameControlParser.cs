using System;
using System.Collections.Generic;

namespace SolStandard.NeoUtility.Controls.Inputs
{
    public class GameControlParser : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlParser(IController controller) : base(controller)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.MoveUp, controller.MoveUp},
                {Input.MoveDown, controller.MoveDown},
                {Input.MoveLeft, controller.MoveLeft},
                {Input.MoveRight, controller.MoveRight},

                {Input.UnusedUp, controller.CameraUp},
                {Input.UnusedDown, controller.CameraDown},
                {Input.UnusedLeft, controller.CameraLeft},
                {Input.UnusedRight, controller.CameraRight},

                {Input.Select, controller.Select},
                {Input.Start, controller.Start},

                {Input.ContextAction, controller.Confirm},
                {Input.TertiaryAction, controller.Cancel},
                {Input.PrimaryAction, controller.SubweaponLeft},
                {Input.SecondaryAction, controller.SubweaponTop},

                {Input.DodgeRoll, controller.TabLeft},
                {Input.Parry, controller.TabRight},
                {Input.UnusedLeftTrigger, controller.LeftTrigger},
                {Input.UnusedRightTrigger, controller.RightTrigger}
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


        public override bool JustReleased(Input input)
        {
            return buttonMap[input].Released;
        }
    }
}