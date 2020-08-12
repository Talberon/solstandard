using System;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.Utility;

namespace SolStandard.Entity.Unit.Actions.Bard
{
    public abstract class SongAction : UnitAction
    {
        protected SongAction(IRenderable icon, string name, string description, SpriteAtlas tileSprite, int[] range,
            bool freeAction)
            : base(
                icon,
                name,
                description + Environment.NewLine + "Removes other songs currently applied.",
                tileSprite,
                range,
                freeAction
            )
        {
        }

        protected bool SingerIsSinging
        {
            get
            {
                GameUnit singer = GlobalContext.Units.FirstOrDefault(unit => unit.Actions.Contains(this));
                return singer != null &&
                       singer.StatusEffects.Any(status => status is SoloStatus || status is ConcertoStatus);
            }
        }
    }
}