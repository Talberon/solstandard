using System;
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

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class FadeStrike : UnitAction
    {
        public FadeStrike() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.FadeStrike, GameDriver.CellSizeVector),
            name: "Fade Strike",
            description: "Move away from an enemy unit as you attack." + Environment.NewLine +
                         "Movement takes place before combat.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actingUnit = GlobalContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Vector2 rearCoordinates = DetermineOppositeTileOfUnit(targetUnit.UnitEntity.MapCoordinates,
                    actingUnit.UnitEntity.MapCoordinates);
                MapSlice oppositeSlice = MapContainer.GetMapSliceAtCoordinates(rearCoordinates);

                if (RearTileIsMovable(oppositeSlice))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(actingUnit.UnitEntity, rearCoordinates));
                    eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new StartCombatEvent(targetUnit));

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Rear tile is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool RearTileIsMovable(MapSlice targetSlice)
        {
            return UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }
    }
}