using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons
{
    public enum PressType
    {
        Repeating,
        Single
    }

    public enum Input
    {
        Up,
        Down,
        Left,
        Right,
        RsUp,
        RsDown,
        RsLeft,
        RsRight,
        A,
        B,
        X,
        Y,
        Select,
        Start,
        LeftBumper,
        LeftTrigger,
        RightBumper,
        RightTrigger
    }

    public class GameControlMapper
    {
        public const float StickThreshold = 0.2f;

        private const int InitialInputDelayInFrames = 15;
        private const int RepeatInputDelayInFrames = 5;

        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlMapper(PlayerIndex playerIndex)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.Up, new UpControl(playerIndex)},
                {Input.Down, new DownControl(playerIndex)},
                {Input.Left, new LeftControl(playerIndex)},
                {Input.Right, new RightControl(playerIndex)},

                {Input.RsUp, new RsUpControl(playerIndex)},
                {Input.RsDown, new RsDownControl(playerIndex)},
                {Input.RsLeft, new RsLeftControl(playerIndex)},
                {Input.RsRight, new RsRightControl(playerIndex)},

                {Input.Start, new StartControl(playerIndex)},
                {Input.Select, new SelectControl(playerIndex)},

                {Input.A, new AControl(playerIndex)},
                {Input.B, new BControl(playerIndex)},
                {Input.X, new XControl(playerIndex)},
                {Input.Y, new YControl(playerIndex)},

                {Input.LeftBumper, new LeftBumperControl(playerIndex)},
                {Input.RightBumper, new RightBumperControl(playerIndex)},
                {Input.LeftTrigger, new LeftTriggerControl(playerIndex)},
                {Input.RightTrigger, new RightTriggerControl(playerIndex)},
            };
        }

        public override string ToString()
        {
            string output = "";

            output += "initialInputDelay: " + InitialInputDelayInFrames;
            output += Environment.NewLine;
            output += "repeatInputDelay: " + RepeatInputDelayInFrames;
            output += Environment.NewLine;
            output += "inputCounters: {";
            output += Environment.NewLine;
            output += " Up: " + buttonMap[Input.Up].InputCounter;
            output += Environment.NewLine;
            output += " Down: " + buttonMap[Input.Down].InputCounter;
            output += Environment.NewLine;
            output += " Left: " + buttonMap[Input.Left].InputCounter;
            output += Environment.NewLine;
            output += " Right: " + buttonMap[Input.Right].InputCounter;
            output += Environment.NewLine;
            output += " A: " + buttonMap[Input.A].InputCounter;
            output += Environment.NewLine;
            output += " B: " + buttonMap[Input.B].InputCounter;
            output += Environment.NewLine;
            output += " X: " + buttonMap[Input.X].InputCounter;
            output += Environment.NewLine;
            output += " Y: " + buttonMap[Input.Y].InputCounter;
            output += Environment.NewLine;
            output += " Select: " + buttonMap[Input.Select].InputCounter;
            output += Environment.NewLine;
            output += " Start: " + buttonMap[Input.Start].InputCounter;
            output += Environment.NewLine;
            output += "}";

            return output;
        }

        public bool Press(Input input, PressType pressType)
        {
            switch (pressType)
            {
                case PressType.Repeating:
                    return InputRepeating(buttonMap[input]);
                case PressType.Single:
                    return InputSingle(buttonMap[input]);
                default:
                    throw new ArgumentOutOfRangeException("pressType", pressType, null);
            }
        }

        private static bool InputSingle(GameControl control)
        {
            //Press just once on input down; do not repeat
            if (control.Pressed())
            {
                if (control.InputCounter == 0)
                {
                    control.IncrementInputCounter();
                    return true;
                }
            }

            if (control.Released())
            {
                control.ResetInputCounter();
            }

            return false;
        }

        private static bool InputRepeating(GameControl control)
        {
            //Hold Down (Previous state matches the current state)
            if (control.Pressed())
            {
                control.IncrementInputCounter();

                //Act on tap
                if (control.InputCounter - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (control.InputCounter > InitialInputDelayInFrames)
                {
                    if (control.InputCounter % RepeatInputDelayInFrames == 0)
                    {
                        return true;
                    }
                }
            }

            if (control.Released())
            {
                control.ResetInputCounter();
            }

            return false;
        }
    }
}