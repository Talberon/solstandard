using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Intervention : UnitAction
    {
        private readonly int blkBonus;
        private readonly int turnDuration;

        public Intervention(int blkBonus, int turnDuration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Intervention, GameDriver.CellSizeVector),
            name: "Intervention",
            description: "Grant a [" + blkBonus + " " + UnitStatistics.Abbreviation[Stats.Block] +
                         "] buff to an ally within range for [" + turnDuration + "] turn(s).",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.blkBonus = blkBonus;
            this.turnDuration = turnDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new BlkStatUp(turnDuration, blkBonus)));
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}