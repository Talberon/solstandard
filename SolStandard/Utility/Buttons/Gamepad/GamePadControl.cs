using Microsoft.Xna.Framework;

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

        protected readonly PlayerIndex PlayerIndex;
        public abstract GamepadInputs InputType { get; }

        protected GamePadControl(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}