using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons
{
    public class GameControlMapper : ControlMapper
    {
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