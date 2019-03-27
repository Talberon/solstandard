using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Replace : UnitAction
    {
        private static readonly int[] SkillRange = {1, 2, 3};

        public Replace() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Blink, new Vector2(GameDriver.CellSize)),
            name: "Replace",
            description: "Swap with another unit within [" + SkillRange.Min() + "-" + SkillRange.Max() + "] spaces.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: SkillRange,
            freeAction: false
        )
        {
        }


        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            RemoveActionTilesOnUntargetableSpaces(mapLayer);
        }

        private static void RemoveActionTilesOnUntargetableSpaces(Layer mapLayer)
        {
            List<MapElement> tilesToRemove = new List<MapElement>();

            foreach (MapElement mapElement in MapContainer.GameGrid[(int) mapLayer])
            {
                if (mapElement == null) continue;

                if (!TargetIsAnotherMovableUnit(MapContainer.GetMapSliceAtCoordinates(mapElement.MapCoordinates)))
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
            if (TargetIsAnotherMovableUnit(targetSlice))
            {
                Vector2 casterCoordinates = GameContext.ActiveUnit.UnitEntity.MapCoordinates;

                UnitEntity caster = GameContext.ActiveUnit.UnitEntity;
                UnitEntity targetUnit = targetSlice.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(caster));
                eventQueue.Enqueue(new HideUnitEvent(targetUnit));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    targetUnit,
                    casterCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(caster));
                eventQueue.Enqueue(new UnhideUnitEvent(targetUnit));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't target here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetIsAnotherMovableUnit(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            return (targetSlice.DynamicEntity != null || targetSlice.PreviewEntity != null) &&
                   targetUnit != null &&
                   targetUnit != GameContext.ActiveUnit &&
                   targetUnit.IsMovable;
        }
    }
}