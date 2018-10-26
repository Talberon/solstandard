using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Archer
{
    public class Draw : UnitAction
    {
        private readonly int statModifier;
        private readonly int duration;

        public Draw(int duration, int statModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Draw, new Vector2(32)),
            name: "Draw",
            description: "Increase own attack range by [+" + statModifier + "] for [" + duration + "] turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0}
        )
        {
            //Add one to the duration to compensate for the counter going down right after the user's turn ends.
            this.duration = duration + 1;
            this.statModifier = statModifier;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastBuffEvent(ref targetUnit, new AtkRangeStatUp(duration, statModifier)));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}