namespace SolStandard.NeoUtility.Controls.Inputs
{
    public enum ControlType
    {
        Keyboard,
        Gamepad
    }

    public enum PressType
    {
        DelayedRepeat,
        InstantRepeat,
        Single
    }

    public enum Input
    {
        None,

        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,

        UnusedUp,
        UnusedDown,
        UnusedLeft,
        UnusedRight,

        ContextAction,
        PrimaryAction,
        SecondaryAction,
        TertiaryAction,

        Start,
        Select,

        DodgeRoll,
        Parry,
        UnusedLeftTrigger,
        UnusedRightTrigger
    }

    public abstract class ControlMapper
    {
        private static ControlType _lastInputType = ControlType.Keyboard;

        public IController Controller { get; }
        private ControlType ControlType => Controller.ControlType;
        private const int InitialInputDelayInFrames = 15;
        private const int RepeatInputDelayInFrames = 5;

        protected ControlMapper(IController controller)
        {
            Controller = controller;
        }

        public abstract bool Press(Input input, PressType pressType);
        public abstract bool Peek(Input input, PressType pressType);
        public abstract bool JustReleased(Input input);

        public bool NotPressed(Input input)
        {
            return !Press(input, PressType.InstantRepeat);
        }

        protected bool InstantRepeat(GameControl control)
        {
            if (control.Pressed) _lastInputType = ControlType;
            return control.Pressed;
        }

        protected bool SinglePress(GameControl control, bool incrementInputCounter)
        {
            //Press just once on input down; do not repeat
            if (control.Pressed)
            {
                if (control.InputCounter == 0)
                {
                    if (incrementInputCounter) control.IncrementInputCounter();
                    _lastInputType = ControlType;
                    return true;
                }
            }

            if (control.Released)
            {
                control.ResetInputCounter();
            }

            return false;
        }

        protected bool DelayedRepeat(GameControl control, bool incrementInputCounter)
        {
            //Hold Down (Previous state matches the current state)
            if (control.Pressed)
            {
                if (incrementInputCounter) control.IncrementInputCounter();

                //Act on tap
                if (control.InputCounter - 1 == 0)
                {
                    _lastInputType = ControlType;
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (control.InputCounter > InitialInputDelayInFrames)
                {
                    if (control.InputCounter % RepeatInputDelayInFrames == 0)
                    {
                        _lastInputType = ControlType;
                        return true;
                    }
                }
            }

            if (control.Released)
            {
                control.ResetInputCounter();
            }

            return false;
        }
    }
}