using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Mage
{
    public class Blink : UnitAction
    {
        private static readonly int[] BlinkRange = {1, 2, 3, 4};

        public Blink() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Blink, new Vector2(32)),
            name: "Blink",
            description: "Move to an unoccupied space within " + BlinkRange.Max() + "spaces.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: BlinkRange
        )
        {
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            RemoveActionTilesOnUnmovableSpaces(mapLayer);
        }

        private static void RemoveActionTilesOnUnmovableSpaces(Layer mapLayer)
        {
            List<MapElement> tilesToRemove = new List<MapElement>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) mapLayer])
            {
                if (mapElement == null) continue;
                if (!UnitMovingContext.CanMoveAtCoordinates(mapElement.MapCoordinates))
                {
                    tilesToRemove.Add(mapElement);
                }
            }

            foreach (MapElement tile in tilesToRemove)
            {
                MapContainer.GameGrid[(int) mapLayer][(int) tile.MapCoordinates.X, (int) tile.MapCoordinates.Y] =
                    null;
            }
        }

        public override void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GameContext.ActiveUnit.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref gameMapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                MapContainer.AddNewToastAtMapCursor("Can't blink here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}