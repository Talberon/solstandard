using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Marauder
{
    public class Brace : UnitAction
    {
        private readonly int duration;

        public Brace(int duration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Brace, GameDriver.CellSizeVector),
            name: "Brace",
            description: "Reduce movement by half and prevent other units from moving this " +
                         "unit with abilities for <" + duration + "> turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                AssetManager.SkillBuffSFX.Play();
                int halfOfUnitsBaseMv = targetUnit.Stats.BaseMv / 2;

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(targetUnit, new MoveStatDown(duration, halfOfUnitsBaseMv))
                );
                eventQueue.Enqueue(new WaitFramesEvent(20));
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new ImmovableStatus(Icon, duration)));
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}