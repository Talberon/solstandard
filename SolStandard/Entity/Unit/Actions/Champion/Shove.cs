using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class Shove : UnitAction
    {
        public Shove() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Shove, GameDriver.CellSizeVector),
            name: "Shove",
            description: "Push a unit away one space if there is an unoccupied space behind them.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                if (CanShove(targetSlice, targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ShoveEvent(targetUnit));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Obstructed/Immovable!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static bool CanShove(MapSlice targetSlice, GameUnit targetUnit)
        {
            Vector2 actorCoordinates = GlobalContext.ActiveUnit.UnitEntity.MapCoordinates;
            Vector2 targetCoordinates = targetUnit.UnitEntity.MapCoordinates;
            Vector2 oppositeCoordinates = DetermineOppositeTileOfUnit(actorCoordinates, targetCoordinates);

            return TargetIsUnitInRange(targetSlice, targetUnit) &&
                   UnitMovingPhase.CanEndMoveAtCoordinates(oppositeCoordinates) &&
                   targetUnit.IsMovable;
        }
    }
}