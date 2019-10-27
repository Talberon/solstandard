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
                {Buttons.LeftThumbstickUp, ButtonIcon.LeftStickUp},
                {Buttons.LeftThumbstickDown, ButtonIcon.LeftStickDown},
                {Buttons.LeftThumbstickLeft, ButtonIcon.LeftStickLeft},
                {Buttons.LeftThumbstickRight, ButtonIcon.LeftStickRight},
                {Buttons.Back, ButtonIcon.Windows},
                {Buttons.Start, ButtonIcon.Menu},
                {Buttons.RightThumbstickUp, ButtonIcon.RightStickUp},
                {Buttons.RightThumbstickDown, ButtonIcon.RightStickDown},
                {Buttons.RightThumbstickLeft, ButtonIcon.RightStickLeft},
                {Buttons.RightThumbstickRight, ButtonIcon.RightStickRight},
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

        public override bool Equals(object obj)
        {
            if (!(obj is InputButton inputButton)) return true;

            if (inputButton.buttons.Length != buttons.Length) return false;

            List<Buttons> theirButtons = inputButton.buttons.OrderBy(b => b).ToList();
            List<Buttons> myButtons = buttons.OrderBy(b => b).ToList();

            return !myButtons.Where((t, i) => theirButtons[i] != t).Any();
        }

        public override int GetHashCode()
        {
            return buttons.Select(button => (int) button ^ 5).Sum();
        }
    }
}