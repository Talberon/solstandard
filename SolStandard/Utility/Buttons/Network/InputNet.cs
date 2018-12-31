using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons.Network
{
    [Serializable]
    public class InputNet : GameControl, ISerializable
    {
        public const string PlayerIndexHeader = "PlayerIndex";

        public enum ControlState
        {
            Pressed,
            Released
        }

        private bool pressed;

        public InputNet(PlayerIndex playerIndex) : base(playerIndex)
        {
            pressed = false;
        }

        public InputNet(SerializationInfo info, StreamingContext context) :
            this((PlayerIndex) info.GetValue(PlayerIndexHeader, typeof(PlayerIndex)))
        {
            pressed = (bool) info.GetValue(ControlState.Pressed.ToString(), typeof(bool));
        }

        public override bool Pressed
        {
            get { return pressed; }
        }

        public override bool Released
        {
            get { return !pressed; }
        }

        public void Press()
        {
            pressed = true;
        }

        public void Release()
        {
            pressed = false;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(PlayerIndexHeader, PlayerIndex);
            info.AddValue(ControlState.Pressed.ToString(), Pressed);
            info.AddValue(ControlState.Released.ToString(), Released);
        }

        public override string ToString()
        {
            return (pressed) ? ControlState.Pressed.ToString() : ControlState.Released.ToString();
        }
    }
}