namespace SolStandard.Utility.Buttons.Network
{
    public class InputNet : GameControl
    {
        public enum ControlState
        {
            Pressed,
            Released
        }

        private bool pressed;

        public InputNet()
        {
            pressed = false;
        }

        public override bool Pressed => pressed;

        public override bool Released => !pressed;

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