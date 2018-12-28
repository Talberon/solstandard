using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Network
{
    public class NetworkControlParser : ControlMapper
    {
        private readonly Dictionary<Input, GameControl> buttonMap;

        private static readonly Dictionary<string, Input> MessageInputMap = new Dictionary<string, Input>
        {
            {"UP", Input.Up},
            {"DOWN", Input.Down},
            {"LEFT", Input.Left},
            {"RIGHT", Input.Right},

            {"RS_UP", Input.RsUp},
            {"RS_DOWN", Input.RsDown},
            {"RS_LEFT", Input.RsLeft},
            {"RS_RIGHT", Input.RsRight},

            {"START", Input.Start},
            {"SELECT", Input.Select},

            {"A", Input.A},
            {"B", Input.B},
            {"X", Input.X},
            {"Y", Input.Y},

            {"LB", Input.LeftBumper},
            {"RB", Input.RightBumper},
            {"LT", Input.LeftTrigger},
            {"RT", Input.RightTrigger},
        };


        public NetworkControlParser(PlayerIndex playerIndex)
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

        public static Input ReadNetworkInput(string message)
        {
            /*
             * TODO Consider turning the Control classes into serializable objects with state that can be sent as
             * objects over network instead of interpreting strings here.
             */
            try
            {
                return MessageInputMap[message];
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.StackTrace);
            }

            return Input.None;
        }

        public override bool Press(Input input, PressType pressType)
        {
            throw new NotImplementedException();
        }

        public override bool Released(Input input)
        {
            throw new NotImplementedException();
        }
    }
}