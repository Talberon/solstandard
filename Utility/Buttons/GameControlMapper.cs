using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SolStandard.Utility.Buttons
{
    class GameControlMapper
    {
        public const float StickThreshold = 0.2f;

        private const int InitialInputDelay = 15;
        private const int RepeatInputDelay = 5;

        private int inputCounterA = 0;
        private int inputCounterB = 0;
        private int inputCounterX = 0;
        private int inputCounterY = 0;
        private int inputCounterLt = 0;
        private int inputCounterRt = 0;
        private int inputCounterBack = 0;
        private int inputCounterStart = 0;

        private GameControl upControl, downControl, leftControl, rightControl;

        public GameControlMapper()
        {
            upControl = new UpControl();
            downControl = new DownControl();
            leftControl = new LeftControl();
            rightControl = new RightControl();
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
            output += " A: " + inputCounterA;
            output += Environment.NewLine;
            output += " B: " + inputCounterB;
            output += Environment.NewLine;
            output += " X: " + inputCounterX;
            output += Environment.NewLine;
            output += " Y: " + inputCounterY;
            output += Environment.NewLine;
            output += " Back: " + inputCounterBack;
            output += Environment.NewLine;
            output += " Start: " + inputCounterStart;
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

        
        //FIXME Continue converting controls to their appropriate classes
        
        //Confirm
        public bool A()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Space)))
            {
                //Increment inputCounter
                inputCounterA++;

                //Act on tap
                if (inputCounterA - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterA > InitialInputDelay)
                {
                    if (inputCounterA % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.Space)))
            {
                //Restart the input counter
                inputCounterA = 0;
            }

            return false;
        }

        //Cancel
        public bool B()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.LeftShift)))
            {
                //Increment inputCounter
                inputCounterB++;

                //Act on tap
                if (inputCounterB - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterB > InitialInputDelay)
                {
                    if (inputCounterB % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.LeftShift)))
            {
                //Restart the input counter
                inputCounterB = 0;
            }

            return false;
        }

        public bool X()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Tab)))
            {
                //Increment inputCounter
                inputCounterX++;

                //Act on tap
                if (inputCounterX - 1 == 0)
                {
                    return true;
                }


                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterX > InitialInputDelay)
                {
                    if (inputCounterX % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.Tab)))
            {
                //Restart the input counter
                inputCounterX = 0;
            }

            return false;
        }

        public bool Y()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.OemTilde)))
            {
                //Increment inputCounter
                inputCounterY++;

                //Act on tap
                if (inputCounterY - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterY > InitialInputDelay)
                {
                    if (inputCounterY % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.OemTilde)))
            {
                //Restart the input counter
                inputCounterY = 0;
            }

            return false;
        }


        public bool LeftTrigger()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Triggers.Left > 0.2f || Keyboard.GetState().IsKeyDown(Keys.Q)))
            {
                //Increment inputCounter
                inputCounterLt++;

                //Act on tap
                if (inputCounterLt - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterLt > InitialInputDelay)
                {
                    if (inputCounterLt % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Triggers.Left == 0f && Keyboard.GetState().IsKeyUp(Keys.Q)))
            {
                //Restart the input counter
                inputCounterLt = 0;
            }

            return false;
        }

        public bool RightTrigger()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Triggers.Right > 0.2f || Keyboard.GetState().IsKeyDown(Keys.E)))
            {
                //Increment inputCounter
                inputCounterRt++;

                //Act on tap
                if (inputCounterRt - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterRt > InitialInputDelay)
                {
                    if (inputCounterRt % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Triggers.Right == 0f && Keyboard.GetState().IsKeyUp(Keys.E)))
            {
                //Restart the input counter
                inputCounterRt = 0;
            }

            return false;
        }


        public bool Start()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Enter)))
            {
                //Increment inputCounter
                inputCounterStart++;

                //Act on tap
                if (inputCounterStart - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterStart > InitialInputDelay)
                {
                    if (inputCounterStart % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.Enter)))
            {
                //Restart the input counter
                inputCounterStart = 0;
            }

            return false;
        }

        public bool Back()
        {
            //Hold Down (Previous state matches the current state)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                 Keyboard.GetState().IsKeyDown(Keys.Escape)))
            {
                //Increment inputCounter
                inputCounterBack++;

                //Act on tap
                if (inputCounterBack - 1 == 0)
                {
                    return true;
                }

                //If the counter is over [initialInputDelay], start tapping every [repeatInputDelay] frames
                if (inputCounterBack > InitialInputDelay)
                {
                    if (inputCounterBack % RepeatInputDelay == 0)
                    {
                        return true;
                    }
                }
            }

            //Let Go (Button is released)
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released &&
                 Keyboard.GetState().IsKeyUp(Keys.Escape)))
            {
                //Restart the input counter
                inputCounterBack = 0;
            }

            return false;
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