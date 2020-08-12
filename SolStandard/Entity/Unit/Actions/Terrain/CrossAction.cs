using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class CrossAction : UnitAction
    {
        private readonly Direction crossDirection;
        private readonly Vector2 crossingOrigin;

        public CrossAction(Vector2 crossingOrigin, Direction crossDirection) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Jump, GameDriver.CellSizeVector),
            name: "Cross",
            description: "Cross the path.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
            this.crossingOrigin = crossingOrigin;
            this.crossDirection = crossDirection;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            Vector2 targetCoordinates = crossingOrigin;
            switch (crossDirection)
            {
                case Direction.None:
                    base.GenerateActionGrid(crossingOrigin, mapLayer);
                    return;
                case Direction.Up:
                    targetCoordinates.Y -= 1;
                    break;
                case Direction.Right:
                    targetCoordinates.X += 1;
                    break;
                case Direction.Down:
                    targetCoordinates.Y += 1;
                    break;
                case Direction.Left:
                    targetCoordinates.X -= 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MapContainer.GameGrid[(int) mapLayer][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(targetCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanCrossPath(targetSlice))
            {
                GlobalEventQueue.QueueSingleEvent(new PlaySoundEffectEvent(AssetManager.MapUnitMoveSFX));
                GlobalEventQueue.QueueSingleEvent(
                    new MoveEntityToCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity, targetSlice.MapCoordinates)
                );
                GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Cannot cross here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool CanCrossPath(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null &&
                   UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }
    }
}