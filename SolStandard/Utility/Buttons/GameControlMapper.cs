using System;
using Microsoft.Xna.Framework;

namespace SolStandard.Utility.Buttons
{
    public class GameControlMapper
    {
        public const float StickThreshold = 0.2f;

        private const int InitialInputDelay = 15;
        private const int RepeatInputDelay = 5;

        private readonly GameControl upControl, downControl, leftControl, rightControl;
        private readonly GameControl rsUpControl, rsDownControl, rsLeftControl, rsRightControl;
        private readonly GameControl aControl, bControl, xControl, yControl;
        private readonly GameControl lbControl, rbControl, ltControl, rtControl;
        private readonly GameControl startControl, selectControl;

        public GameControlMapper(PlayerIndex playerIndex)
        {
            upControl = new UpControl(playerIndex);
            downControl = new DownControl(playerIndex);
            leftControl = new LeftControl(playerIndex);
            rightControl = new RightControl(playerIndex);

            rsUpControl = new RsUpControl(playerIndex);
            rsDownControl = new RsDownControl(playerIndex);
            rsLeftControl = new RsLeftControl(playerIndex);
            rsRightControl = new RsRightControl(playerIndex);

            startControl = new StartControl(playerIndex);
            selectControl = new SelectControl(playerIndex);

            aControl = new AControl(playerIndex);
            bControl = new BControl(playerIndex);
            xControl = new XControl(playerIndex);
            yControl = new YControl(playerIndex);

            lbControl = new LeftBumperControl(playerIndex);
            rbControl = new RightBumperControl(playerIndex);
            ltControl = new LeftTriggerControl(playerIndex);
            rtControl = new RightTriggerControl(playerIndex);
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

        public bool RightStickUp()
        {
            return CheckInputDelay(rsUpControl);
        }

        public bool RightStickDown()
        {
            return CheckInputDelay(rsDownControl);
        }

        public bool RightStickLeft()
        {
            return CheckInputDelay(rsLeftControl);
        }

        public bool RightStickRight()
        {
            return CheckInputDelay(rsRightControl);
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

        public bool LeftBumper()
        {
            return CheckInputDelay(lbControl);
        }

        public bool RightBumper()
        {
            return CheckInputDelay(rbControl);
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