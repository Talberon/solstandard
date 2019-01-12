namespace SolStandard.Utility.Buttons
{
    public enum PressType
    {
        DelayedRepeat,
        InstantRepeat,
        Single
    }

    public enum Input
    {
        None,
        CursorUp,
        CursorDown,
        CursorLeft,
        CursorRight,
        CameraUp,
        CameraDown,
        CameraLeft,
        CameraRight,
        Confirm,
        Cancel,
        ResetToUnit,
        CenterCamera,
        Status,
        Menu,
        LeftBumper,
        LeftTrigger,
        RightBumper,
        RightTrigger
    }

    public abstract class ControlMapper
    {
        public const float StickDeadzone = 0.2f;
        public const float TriggerDeadzone = 0.2f;
        protected const int InitialInputDelayInFrames = 15;
        protected const int RepeatInputDelayInFrames = 5;

        public abstract bool Press(Input input, PressType pressType);
        public abstract bool Peek(Input input, PressType pressType);
        public abstract bool Released(Input input);

        protected static bool InstantRepeat(GameControl control)
        {
            return control.Pressed;
        }

        protected static bool SinglePress(GameControl control, bool incrementInputCounter)
        {
            //Press just once on input down; do not repeat
            if (control.Pressed)
            {
                if (control.InputCounter == 0)
                {
                    if (incrementInputCounter) control.IncrementInputCounter();
                    return true;
                }
            }

            if (control.Released)
            {
                control.ResetInputCounter();
            }

            return false;
        }

        protected static bool DelayedRepeat(GameControl control, bool incrementInputCounter)
        {
            //Hold Down (Previous state matches the current state)
            if (control.Pressed)
            {
                if (incrementInputCounter) control.IncrementInputCounter();

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

            if (control.Released)
            {
                control.ResetInputCounter();
            }

            return false;
        }
    }
}