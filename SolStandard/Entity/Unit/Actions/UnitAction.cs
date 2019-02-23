using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions
{
    public abstract class UnitAction
    {
        public IRenderable Icon { get; private set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        protected readonly SpriteAtlas TileSprite;
        public int[] Range { get; protected set; }

        protected UnitAction(IRenderable icon, string name, string description, SpriteAtlas tileSprite, int[] range)
        {
            Icon = icon;
            Name = name;
            Description = description;
            TileSprite = tileSprite;
            Range = range;
        }

        public virtual void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(origin);
        }

        public abstract void ExecuteAction(MapSlice targetSlice);

        public void CancelAction()
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            AssetManager.MapUnitCancelSFX.Play();
        }

        protected static bool TargetIsUnitInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                targetUnit != null
                && targetSlice.DynamicEntity != null;
        }

        protected static bool TargetIsAnAllyInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                TargetIsUnitInRange(targetSlice, targetUnit)
                && targetUnit.Team == GameContext.ActiveUnit.Team;
        }

        protected static bool TargetIsAnEnemyInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                TargetIsUnitInRange(targetSlice, targetUnit)
                && GameContext.ActiveUnit.Team != targetUnit.Team;
        }

        protected static bool TargetIsSelfInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                TargetIsUnitInRange(targetSlice, targetUnit)
                && GameContext.ActiveUnit == targetUnit;
        }

        protected static bool TargetIsABreakableObstacleInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.TerrainEntity is BreakableObstacle;
        }

        protected static bool CanMoveToTargetTile(MapSlice targetSlice)
        {
            return UnitMovingContext.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.DynamicEntity != null;
        }

        protected static bool SourceSouthOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.Y > targetCoordinates.Y;
        }

        protected static bool SourceWestOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.X < targetCoordinates.X;
        }

        protected static bool SourceEastOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.X > targetCoordinates.X;
        }

        protected static bool SourceNorthOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.Y < targetCoordinates.Y;
        }

        public static Vector2 DetermineOppositeTileOfUnit(Vector2 actorCoordinates, Vector2 targetCoordinates)
        {
            Vector2 oppositeCoordinates = targetCoordinates;

            if (SourceNorthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move South
                oppositeCoordinates.Y++;
            }

            if (SourceSouthOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move North
                oppositeCoordinates.Y--;
            }

            if (SourceEastOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move West
                oppositeCoordinates.X--;
            }

            if (SourceWestOfTarget(actorCoordinates, targetCoordinates))
            {
                //Move East
                oppositeCoordinates.X++;
            }

            return oppositeCoordinates;
        }
    }
}