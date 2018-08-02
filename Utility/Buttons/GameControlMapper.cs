using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    public class GameControlMapper
    {
        public const float StickThreshold = 0.2f;

        private const int InitialInputDelay = 15;
        private const int RepeatInputDelay = 5;

        private readonly GameControl upControl, downControl, leftControl, rightControl;
        private readonly GameControl aControl, bControl, xControl, yControl;
        private readonly GameControl ltControl, rtControl;
        private readonly GameControl startControl, selectControl;

        public GameControlMapper()
        {
            upControl = new UpControl();
            downControl = new DownControl();
            leftControl = new LeftControl();
            rightControl = new RightControl();

            this.startControl = new StartControl();
            this.selectControl = new SelectControl();

            this.aControl = new AControl();
            this.bControl = new BControl();
            this.xControl = new XControl();
            this.yControl = new YControl();

            this.ltControl = new LeftTriggerControl();
            this.rtControl = new RightTriggerControl();
        }

        public override string ToString()
        {
            string output = "";

            output += "initialInputDelay: " + InitialInputDelay;
            output += Environment.NewLine;
            output += "repeatInputDelay: " + RepeatInputDelay;
            output += Environment.NewLine;
            output += "inputCounters: {";
            output += Environment.NewLine;
            output += " Up: " + upControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Down: " + downControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Left: " + leftControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Right: " + rightControl.GetInputCounter();
            output += Environment.NewLine;
            output += " A: " + aControl.GetInputCounter();
            output += Environment.NewLine;
            output += " B: " + bControl.GetInputCounter();
            output += Environment.NewLine;
            output += " X: " + xControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Y: " + yControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Select: " + selectControl.GetInputCounter();
            output += Environment.NewLine;
            output += " Start: " + startControl.GetInputCounter();
            output += Environment.NewLine;
            output += "}";

            return output;
        }


        public bool Up()
        {
            return CheckInputDelay(upControl);
        }

        public bool Down()
        {
            return CheckInputDelay(downControl);
        }

        public bool Left()
        {
            return CheckInputDelay(leftControl);
        }

        public bool Right()
        {
            return CheckInputDelay(rightControl);
        }

        public bool A()
        {
            return CheckInputDelay(aControl);
        }

        public bool B()
        {
            return CheckInputDelay(bControl);
        }

        public bool X()
        {
            return CheckInputDelay(xControl);
        }

        public bool Y()
        {
            return CheckInputDelay(yControl);
        }

        public bool LeftTrigger()
        {
            return CheckInputDelay(ltControl);
        }

        public bool RightTrigger()
        {
            return CheckInputDelay(rtControl);
        }

        public bool Start()
        {
            return CheckInputDelay(startControl);
        }

        public bool Select()
        {
            return CheckInputDelay(selectControl);
        }

        private static bool CheckInputDelay(GameControl control)
        {
            //Hold Down (Previous state matches the current state)
            if (control.Pressed())
            {
                control.IncrementInputCounter();

                //Act on tap
                if (control.GetInputCounter() - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (control.GetInputCounter() > InitialInputDelay)
                {
                    if (control.GetInputCounter() % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            if (control.Released())
            {
                control.ResetInputCounter();
            }

            return false;
        }
    }
}