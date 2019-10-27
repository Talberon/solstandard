using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
{
    public abstract class GamePadControl : GameControl
    {
        public enum GamepadInputs
        {
            A,
            B,
            X,
            Y,

            Up,
            Down,
            Left,
            Right,

            Select,
            Start,

            RightStickUp,
            RightStickDown,
            RightStickLeft,
            RightStickRight,

            Lb,
            Lt,
            Rb,
            Rt,
        }

        private static readonly Dictionary<GamepadInputs, ButtonIcon> ButtonIcons =
            new Dictionary<GamepadInputs, ButtonIcon>
            {
                {GamepadInputs.A, ButtonIcon.A},
                {GamepadInputs.B, ButtonIcon.B},
                {GamepadInputs.X, ButtonIcon.X},
                {GamepadInputs.Y, ButtonIcon.Y},
                {GamepadInputs.Up, ButtonIcon.DpadUp},
                {GamepadInputs.Down, ButtonIcon.DpadDown},
                {GamepadInputs.Left, ButtonIcon.DpadLeft},
                {GamepadInputs.Right, ButtonIcon.DpadRight},
                {GamepadInputs.Select, ButtonIcon.Windows},
                {GamepadInputs.Start, ButtonIcon.Menu},
                {GamepadInputs.RightStickUp, ButtonIcon.RightStick},
                {GamepadInputs.RightStickDown, ButtonIcon.RightStick},
                {GamepadInputs.RightStickLeft, ButtonIcon.RightStick},
                {GamepadInputs.RightStickRight, ButtonIcon.RightStick},
                {GamepadInputs.Lb, ButtonIcon.Lb},
                {GamepadInputs.Lt, ButtonIcon.Lt},
                {GamepadInputs.Rb, ButtonIcon.Rb},
                {GamepadInputs.Rt, ButtonIcon.Rt},
            };

        protected readonly PlayerIndex PlayerIndex;
        public abstract GamepadInputs InputType { get; }

        public override IRenderable GetInputIcon(int iconSize)
        {
            return ButtonIconProvider.GetButton(ButtonIcons[InputType], new Vector2(iconSize));
        }
        
        protected GamePadControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}