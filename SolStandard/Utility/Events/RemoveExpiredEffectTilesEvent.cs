using System.Collections.Generic;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;

namespace SolStandard.Utility.Events
{
    public class RemoveExpiredEffectTilesEvent : IEvent
    {
        private readonly List<IEffectTile> effectTiles;

        public RemoveExpiredEffectTilesEvent(List<IEffectTile> effectTiles)
        {
            this.effectTiles = effectTiles;
        }

        public bool Complete { get; private set; }

        public void Continue()
        {
            GameMapContext.RemoveExpiredEffectTiles(effectTiles);
            Complete = true;
        }
    }
}