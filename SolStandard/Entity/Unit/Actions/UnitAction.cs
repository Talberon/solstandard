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
        public string Name { get; private set; }
        public string Description { get; private set; }
        protected readonly SpriteAtlas TileSprite;
        public int[] Range { get; private set; }

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
        }

        public abstract void ExecuteAction(MapSlice targetSlice, GameMapContext gameMapContext, BattleContext battleContext);

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void CancelAction(GameMapContext gameMapContext)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            gameMapContext.RevertToPreviousState();
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
            return targetSlice.DynamicEntity != null &&
                   targetSlice.TerrainEntity != null &&
                   targetSlice.TerrainEntity.GetType() == typeof(BreakableObstacle);
        }

        protected static bool CanMoveToTargetTile(MapSlice targetSlice)
        {
            return UnitMovingContext.CanMoveAtCoordinates(targetSlice.MapCoordinates) &&
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
    }
}