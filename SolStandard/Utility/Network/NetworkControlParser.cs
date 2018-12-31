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
            {"UP", Input.CursorUp},
            {"DOWN", Input.CursorDown},
            {"LEFT", Input.CursorLeft},
            {"RIGHT", Input.CursorRight},

            {"RS_UP", Input.CameraUp},
            {"RS_DOWN", Input.CameraDown},
            {"RS_LEFT", Input.CameraLeft},
            {"RS_RIGHT", Input.CameraRight},

            {"START", Input.Menu},
            {"SELECT", Input.Status},

            {"A", Input.Confirm},
            {"B", Input.Cancel},
            {"X", Input.ResetToUnit},
            {"Y", Input.CenterCamera},

            {"LB", Input.LeftBumper},
            {"RB", Input.RightBumper},
            {"LT", Input.LeftTrigger},
            {"RT", Input.RightTrigger},
        };


        public NetworkControlParser(PlayerIndex playerIndex)
        {
            buttonMap = new Dictionary<Input, GameControl>
            {
                {Input.CursorUp, new GamepadUp(playerIndex)},
                {Input.CursorDown, new GamepadDown(playerIndex)},
                {Input.CursorLeft, new GamepadLeft(playerIndex)},
                {Input.CursorRight, new GamepadRight(playerIndex)},

                {Input.CameraUp, new GamepadRsUp(playerIndex)},
                {Input.CameraDown, new GamepadRsDown(playerIndex)},
                {Input.CameraLeft, new GamepadRsLeft(playerIndex)},
                {Input.CameraRight, new GamepadRsRight(playerIndex)},

                {Input.Menu, new GamepadStart(playerIndex)},
                {Input.Status, new GamepadSelect(playerIndex)},

                {Input.Confirm, new GamepadA(playerIndex)},
                {Input.Cancel, new GamepadB(playerIndex)},
                {Input.ResetToUnit, new GamepadX(playerIndex)},
                {Input.CenterCamera, new GamepadY(playerIndex)},

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