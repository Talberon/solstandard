using SolStandard.Utility.Assets;

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
        PreviewUnit,
        PreviewItem,
        Status,
        Menu,
        TabLeft,
        TabRight,
        ZoomOut,
        ZoomIn
    }

    public abstract class ControlMapper
    {
        public readonly ControlType ControlType;
        public const float StickDeadzone = 0.2f;
        public const float TriggerDeadzone = 0.2f;
        private const int InitialInputDelayInFrames = 15;
        private const int RepeatInputDelayInFrames = 5;

        protected ControlMapper(ControlType controlType)
        {
            ControlType = controlType;
        }

        public abstract bool Press(Input input, PressType pressType);
        public abstract bool Peek(Input input, PressType pressType);
        public abstract bool Released(Input input);

        protected bool InstantRepeat(GameControl control)
        {
            if (control.Pressed) InputIconProvider.UpdateLastInputType(ControlType);
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
                    InputIconProvider.UpdateLastInputType(ControlType);
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
                    InputIconProvider.UpdateLastInputType(ControlType);
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (control.InputCounter > InitialInputDelayInFrames)
                {
                    if (control.InputCounter % RepeatInputDelayInFrames == 0)
                    {
                        InputIconProvider.UpdateLastInputType(ControlType);
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