using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Buttons.Network
{
    public class InputNet : GameControl
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

        public override string ToString()
        {
            return (pressed) ? ControlState.Pressed.ToString() : ControlState.Released.ToString();
        }
    }
}