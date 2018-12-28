using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons;

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