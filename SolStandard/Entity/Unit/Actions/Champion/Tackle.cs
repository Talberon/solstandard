using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Tackle : UnitAction
    {
        public Tackle() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Tackle, GameDriver.CellSizeVector),
            name: "Tackle",
            description: "Shove an enemy if there is an empty space behind them,"
                         + "\nthen follow up by moving into their space and attacking.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.CanShove(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ShoveEvent(targetUnit));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(
                        new MoveEntityToCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity, targetOriginalPosition)
                    );
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}