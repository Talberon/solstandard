using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity.General.Item;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Blink : UnitAction
    {
        private BlinkItem Item { get; }

        public Blink(BlinkItem item) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Blink, GameDriver.CellSizeVector),
            name: "Blink: " + item.Name,
            description: "Move to an unoccupied space within [" + item.BlinkRange.Min() + "-" + item.BlinkRange.Max() +
                         "] spaces." + Environment.NewLine + "Uses Remaining: [" + item.UsesRemaining + "]",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: item.BlinkRange,
            freeAction: false
        )
        {
            Item = item;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var unitTargetingContext = new UnitTargetingPhase(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            RemoveActionTilesOnUnmovableSpaces(mapLayer);
        }

        public static void RemoveActionTilesOnUnmovableSpaces(Layer mapLayer)
        {
            var tilesToRemove = new List<MapElement>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) mapLayer])
            {
                if (mapElement == null) continue;
                if (!UnitMovingPhase.CanEndMoveAtCoordinates(mapElement.MapCoordinates))
                {
                    tilesToRemove.Add(mapElement);
                }
            }

            foreach (MapElement tile in tilesToRemove)
            {
                MapContainer.GameGrid[(int) mapLayer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] = null;
            }
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (Item.UsesRemaining > 0)
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    UnitEntity targetEntity = GlobalContext.ActiveUnit.UnitEntity;

                    MapContainer.ClearDynamicAndPreviewGrids();

                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(
                        new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetEntity.MapCoordinates)
                    );
                    eventQueue.Enqueue(new HideUnitEvent(targetEntity));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new BlinkCoordinatesEvent(
                        GlobalContext.ActiveUnit.UnitEntity,
                        targetSlice.MapCoordinates
                    ));
                    eventQueue.Enqueue(new UnhideUnitEvent(targetEntity));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't blink here!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Item has no uses remaining!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}