using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Terraform : UnitAction
    {
        public Terraform() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Terraform, GameDriver.CellSizeVector),
            name: "Geomancy - Terraform",
            description: "Raise a piece of destructible terrain within range.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2},
            freeAction: false
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var unitTargetingContext = new UnitTargetingPhase(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            RemoveActionTilesOnUnplaceableSpaces(mapLayer);
        }

        private static void RemoveActionTilesOnUnplaceableSpaces(Layer mapLayer)
        {
            var tilesToRemove = new List<MapElement>();
            List<MapElement> targetTiles = MapContainer.GetMapElementsFromLayer(mapLayer);

            foreach (MapElement element in targetTiles)
            {
                MapSlice elementSlice = MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates);
                if (!TargetIsUnoccupiedTileInRange(elementSlice))
                {
                    tilesToRemove.Add(element);
                }
            }

            foreach (MapElement tile in tilesToRemove)
            {
                MapContainer.GameGrid[(int) mapLayer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = null;
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsUnoccupiedTileInRange(targetSlice))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                BreakableObstacle rubble = GenerateObstacle(targetSlice.MapCoordinates);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(new PlaceEntityOnMapEvent(rubble, Layer.Entities, AssetManager.CombatBlockSFX));
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer
                    .AddNewToastAtMapCursor("Must target unoccupied tile in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private BreakableObstacle GenerateObstacle(Vector2 mapCoordinates)
        {
            return new BreakableObstacle("Rubble", "BreakableObstacle", Icon, mapCoordinates, 1, false, false, 0);
        }

        private static bool TargetIsUnoccupiedTileInRange(MapSlice targetSlice)
        {
            return UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   (targetSlice.DynamicEntity != null || targetSlice.PreviewEntity != null) &&
                   targetSlice.CollideTile == null && targetSlice.TerrainEntity == null &&
                   targetSlice.UnitEntity == null;
        }
    }
}