using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Buttons.Gamepad;

namespace SolStandard.Utility.Network
{
    public class NetworkControlParser : ControlMapper
    {
        public static Input ReadNetworkInput(string message)
        {
            /*
             * TODO Consider turning the Control classes into serializable objects with state that can be sent as
             * objects over network instead of interpreting strings here.
             */
            try
            {
                return Input.None;
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