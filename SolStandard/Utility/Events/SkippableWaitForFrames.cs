using Microsoft.Xna.Framework;
using SolStandard.Utility.Inputs;

namespace SolStandard.Utility.Events
{
    public class SkippableWaitForFrames : IEvent
    {
        public bool Complete { get; private set; }
        private int framesRemaining;

        public SkippableWaitForFrames(int waitTimeInFrames)
        {
            Complete = false;
            framesRemaining = waitTimeInFrames;
        }

        public void Continue()
        {
            if (!GameDriver.ConnectedAsClient && !GameDriver.ConnectedAsServer)
            {
                ControlMapper p1Input = GameDriver.GetControlMapperForPlayer(PlayerIndex.One);
                ControlMapper p2Input = GameDriver.GetControlMapperForPlayer(PlayerIndex.Two);
                if (p1Input.Press(Input.Confirm, PressType.InstantRepeat) ||
                    p2Input.Press(Input.Confirm, PressType.InstantRepeat))
                {
                    framesRemaining = 0;
                }
            }

            if (framesRemaining > 0)
            {
                framesRemaining--;
            }
            else
            {
                Complete = true;
            }
        }
    }
}