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
    public class Fleche : UnitAction
    {
        private enum ActionPhase
        {
            SelectTarget,
            SelectPlacementSpace
        }

        private ActionPhase currentPhase = ActionPhase.SelectTarget;

        public Fleche() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Fleche, GameDriver.CellSizeVector),
            name: "Fleche",
            description: "Move to any side of a target unit in range." + Environment.NewLine +
                         $"Cannot be used if {UnitStatistics.Abbreviation[Stats.Mv]} is less than 1.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            switch (currentPhase)
            {
                case ActionPhase.SelectTarget:
                    if (SelectTarget(targetSlice)) currentPhase = ActionPhase.SelectPlacementSpace;
                    break;
                case ActionPhase.SelectPlacementSpace:
                    if (SelectDestination(targetSlice)) currentPhase = ActionPhase.SelectTarget;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void CancelAction()
        {
            currentPhase = ActionPhase.SelectTarget;
            base.CancelAction();
        }

        private bool SelectTarget(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                bool unitCanMove = GlobalContext.ActiveUnit.Stats.Mv > 1;

                if (unitCanMove)
                {
                    MapContainer.ClearDynamicAndPreviewGrids();
                    AssetManager.MenuConfirmSFX.Play();
                    GeneratePlacementTiles(targetUnit.UnitEntity.MapCoordinates);
                    return true;
                }

                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot move!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target unit in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private static bool SelectDestination(MapSlice targetSlice)
        {
            if (TargetTileCanPlaceUnit(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(
                    new MoveEntityToCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.CombatDamageSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
                return true;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must place unit in unoccupied space!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private static bool TargetTileCanPlaceUnit(MapSlice targetSlice)
        {
            return UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.DynamicEntity != null;
        }

        private void GeneratePlacementTiles(Vector2 origin)
        {
            base.GenerateActionGrid(origin);

            List<MapElement> targetTiles = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);

            foreach (MapElement tile in targetTiles)
            {
                MapSlice tileSlice = MapContainer.GetMapSliceAtCoordinates(tile.MapCoordinates);
                if (!TargetTileCanPlaceUnit(tileSlice))
                {
                    MapContainer.GameGrid[(int) Layer.Dynamic][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y]
                        = null;
                }
            }
        }
    }
}