using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons
{
    public class GameControlMapper : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        public GameControlMapper(PlayerIndex playerIndex)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.Up, new GamepadUp(playerIndex)},
                {Input.Down, new GamepadDown(playerIndex)},
                {Input.Left, new GamepadLeft(playerIndex)},
                {Input.Right, new GamepadRight(playerIndex)},

                {Input.RsUp, new GamepadRsUp(playerIndex)},
                {Input.RsDown, new GamepadRsDown(playerIndex)},
                {Input.RsLeft, new GamepadRsLeft(playerIndex)},
                {Input.RsRight, new GamepadRsRight(playerIndex)},

                {Input.Start, new GamepadStart(playerIndex)},
                {Input.Select, new GamepadSelect(playerIndex)},

                {Input.A, new GamepadA(playerIndex)},
                {Input.B, new GamepadB(playerIndex)},
                {Input.X, new GamepadX(playerIndex)},
                {Input.Y, new GamepadY(playerIndex)},

                {Input.LeftBumper, new GamepadLeftBumper(playerIndex)},
                {Input.RightBumper, new GamepadRightBumper(playerIndex)},
                {Input.LeftTrigger, new GamepadLeftTrigger(playerIndex)},
                {Input.RightTrigger, new GamepadRightTrigger(playerIndex)},
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
            return (buttonMap[input].Released);
        }
    }
}