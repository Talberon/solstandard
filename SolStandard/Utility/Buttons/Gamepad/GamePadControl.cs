using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Assets;

namespace SolStandard.Utility.Buttons.Gamepad
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

    public abstract class GamePadControl : GameControl
    {
        public static readonly IReadOnlyDictionary<GamepadInputs, ButtonIcon> ButtonIcons =
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

    public static class GamepadControlFactory
    {
        public static GamePadControl GetGamepadControl(PlayerIndex playerIndex, GamepadInputs inputType)
        {
            switch (inputType)
            {
                case GamepadInputs.A:
                    return new GamepadA(playerIndex);
                case GamepadInputs.B:
                    return new GamepadB(playerIndex);
                case GamepadInputs.X:
                    return new GamepadX(playerIndex);
                case GamepadInputs.Y:
                    return new GamepadY(playerIndex);
                case GamepadInputs.Up:
                    return new GamepadUp(playerIndex);
                case GamepadInputs.Down:
                    return new GamepadDown(playerIndex);
                case GamepadInputs.Left:
                    return new GamepadLeft(playerIndex);
                case GamepadInputs.Right:
                    return new GamepadRight(playerIndex);
                case GamepadInputs.Select:
                    return new GamepadSelect(playerIndex);
                case GamepadInputs.Start:
                    return new GamepadStart(playerIndex);
                case GamepadInputs.RightStickUp:
                    return new GamepadRsUp(playerIndex);
                case GamepadInputs.RightStickDown:
                    return new GamepadRsDown(playerIndex);
                case GamepadInputs.RightStickLeft:
                    return new GamepadRsLeft(playerIndex);
                case GamepadInputs.RightStickRight:
                    return new GamepadRsRight(playerIndex);
                case GamepadInputs.Lb:
                    return new GamepadLeftBumper(playerIndex);
                case GamepadInputs.Lt:
                    return new GamepadLeftTrigger(playerIndex);
                case GamepadInputs.Rb:
                    return new GamepadRightBumper(playerIndex);
                case GamepadInputs.Rt:
                    return new GamepadRightTrigger(playerIndex);
                default:
                    throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null);
            }
        }
    }
}