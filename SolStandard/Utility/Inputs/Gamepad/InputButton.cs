using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Inputs.Gamepad
{
    public class InputButton : GameControl
    {
        public static readonly IReadOnlyDictionary<Buttons, ButtonIcon> ButtonIcons =
            new Dictionary<Buttons, ButtonIcon>
            {
                {Buttons.A, ButtonIcon.A},
                {Buttons.B, ButtonIcon.B},
                {Buttons.X, ButtonIcon.X},
                {Buttons.Y, ButtonIcon.Y},
                {Buttons.DPadUp, ButtonIcon.DpadUp},
                {Buttons.DPadDown, ButtonIcon.DpadDown},
                {Buttons.DPadLeft, ButtonIcon.DpadLeft},
                {Buttons.DPadRight, ButtonIcon.DpadRight},
                {Buttons.LeftThumbstickUp, ButtonIcon.LeftStick},
                {Buttons.LeftThumbstickDown, ButtonIcon.LeftStick},
                {Buttons.LeftThumbstickLeft, ButtonIcon.LeftStick},
                {Buttons.LeftThumbstickRight, ButtonIcon.LeftStick},
                {Buttons.Back, ButtonIcon.Windows},
                {Buttons.Start, ButtonIcon.Menu},
                {Buttons.RightThumbstickUp, ButtonIcon.RightStick},
                {Buttons.RightThumbstickDown, ButtonIcon.RightStick},
                {Buttons.RightThumbstickLeft, ButtonIcon.RightStick},
                {Buttons.RightThumbstickRight, ButtonIcon.RightStick},
                {Buttons.LeftShoulder, ButtonIcon.Lb},
                {Buttons.LeftTrigger, ButtonIcon.Lt},
                {Buttons.RightShoulder, ButtonIcon.Rb},
                {Buttons.RightTrigger, ButtonIcon.Rt},
            };

        private readonly PlayerIndex playerIndex;
        private readonly Buttons[] buttons;

        public override bool Pressed => buttons.Any(button => GamePad.GetState(playerIndex).IsButtonDown(button));

        public InputButton(PlayerIndex playerIndex, params Buttons[] buttons)
        {
            this.playerIndex = playerIndex;
            this.buttons = buttons;
        }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[buttons.First()], new Vector2(iconSize));
        }
    }
}