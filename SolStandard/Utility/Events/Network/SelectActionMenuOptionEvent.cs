using System;
using SolStandard.Containers.Components.Global;

namespace SolStandard.Utility.Events.Network
{
    [Serializable]
    public class SelectActionMenuOptionEvent : NetworkEvent
    {
        public override void Continue()
        {
            GameContext.GameMapContext.SelectActionMenuOption();
            Complete = true;
        }
    }
}