using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Inferno : LayTrap
    {
        public Inferno(int damage, int maxTriggers) : base(
            skillIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Inferno, new Vector2(GameDriver.CellSize)),
            trapSprite: new AnimatedSpriteSheet(
                AssetManager.FireTexture, AssetManager.FireTexture.Height, new Vector2(GameDriver.CellSize), 6, false,
                Color.White
            ),
            title: "Pyromancy - Inferno",
            damage: damage,
            maxTriggers: maxTriggers,
            description: "Place up to 4 traps around you that will deal [" + damage +
                         "] damage to enemies that start their turn on it." + Environment.NewLine +
                         "Max activations: [" + maxTriggers + "]",
            freeAction: true
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            RemoveActionTilesOnUnplaceableSpaces(mapLayer);
        }

        private static void RemoveActionTilesOnUnplaceableSpaces(Layer mapLayer)
        {
            List<MapElement> tilesToRemove = new List<MapElement>();

            List<MapElement> targetTiles = MapContainer.GetMapElementsFromLayer(mapLayer);

            foreach (MapElement element in targetTiles)
            {
                if (TargetHasEntityOrWall(MapContainer.GetMapSliceAtCoordinates(element.MapCoordinates)))
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
            if (TargetIsInRange(targetSlice))
            {
                bool allTilesObstructed = true;

                Queue<IEvent> eventQueue = new Queue<IEvent>();

                foreach (MapElement targetTile in MapContainer.GetMapElementsFromLayer(Layer.Dynamic))
                {
                    MapSlice slice = MapContainer.GetMapSliceAtCoordinates(targetTile.MapCoordinates);

                    if (TargetHasEntityOrWall(slice)) continue;

                    TrapEntity trap = new TrapEntity("Fire", TrapSprite.Clone(), slice.MapCoordinates, Damage,
                        MaxTriggers, true, true);

                    MapContainer.ClearDynamicAndPreviewGrids();

                    eventQueue.Enqueue(new PlaceEntityOnMapEvent(trap, Layer.Entities, AssetManager.DropItemSFX));
                    allTilesObstructed = false;
                }

                if (allTilesObstructed)
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("All tiles are obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
                else
                {
                    eventQueue.Enqueue(new WaitFramesEvent(30));
                    eventQueue.Enqueue(new AdditionalActionEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}