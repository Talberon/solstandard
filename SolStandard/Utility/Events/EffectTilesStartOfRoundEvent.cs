using SolStandard.Containers.Components.World;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class EffectTilesStartOfRoundEvent : IEvent
    {
        public bool Complete { get; private set; }

        public void Continue()
        {
            //IMPORTANT Do not allow tiles that have been triggered to trigger again or the risk of soft-locking via infinite triggers can occur
            GameMapContext.TriggerEffectTiles(EffectTriggerTime.StartOfRound, false);
            Complete = true;
        }
    }
}