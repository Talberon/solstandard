using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.General;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions
{
    public abstract class UnitAction
    {
        public IRenderable Icon { get; }
        public string Name { get; protected set; }
        public IRenderable Description { get; protected set; }
        protected readonly SpriteAtlas TileSprite;
        public int[] Range { get; protected set; }
        public bool FreeAction { get; }

        protected UnitAction(IRenderable icon, string name, IRenderable description, SpriteAtlas tileSprite,
            int[] range, bool freeAction)
        {
            Icon = icon;
            Name = name;
            Description = description;
            TileSprite = tileSprite;
            Range = range;
            FreeAction = freeAction;
        }

        protected UnitAction(IRenderable icon, string name, string description, SpriteAtlas tileSprite,
            int[] range, bool freeAction) : this(icon, name, DescriptionRenderText(description), tileSprite, range,
            freeAction)
        {
        }

        private static RenderText DescriptionRenderText(string description)
        {
            return new RenderText(AssetManager.WindowFont, description);
        }

        public virtual void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var unitTargetingContext = new UnitTargetingPhase(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(origin);
        }

        public abstract void ExecuteAction(MapSlice targetSlice);

        public virtual void CancelAction()
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
                && (targetUnit.Team == GlobalContext.ActiveTeam || TargetIsACoOpAlly(targetUnit));
        }

        protected static bool TargetIsAnEnemyInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                TargetIsUnitInRange(targetSlice, targetUnit)
                && GlobalContext.ActiveTeam != targetUnit.Team
                && !TargetIsACoOpAlly(targetUnit);
        }

        protected static bool TargetIsSelfInRange(MapSlice targetSlice, GameUnit targetUnit)
        {
            return
                TargetIsUnitInRange(targetSlice, targetUnit)
                && GlobalContext.ActiveUnit == targetUnit;
        }

        protected static bool TargetIsACoOpAlly(GameUnit targetUnit)
        {
            return GlobalContext.Scenario.Objectives.ContainsKey(VictoryConditions.CollectTheRelicsCoOp) &&
                   targetUnit.Team != Team.Creep;
        }

        protected static bool TargetIsABreakableObstacleInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.TerrainEntity is BreakableObstacle;
        }

        protected static bool CanMoveToTargetTile(MapSlice targetSlice)
        {
            return UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates) &&
                   targetSlice.DynamicEntity != null;
        }

        public static bool SourceSouthOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.Y > targetCoordinates.Y;
        }

        public static bool SourceWestOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.X < targetCoordinates.X;
        }

        public static bool SourceEastOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.X > targetCoordinates.X;
        }

        public static bool SourceNorthOfTarget(Vector2 sourceCoordinates, Vector2 targetCoordinates)
        {
            return sourceCoordinates.Y < targetCoordinates.Y;
        }

        public static Vector2 DetermineOppositeTileOfUnit(Vector2 movingFrom, Vector2 movingAcross)
        {
            Vector2 oppositeCoordinates = movingAcross;

            if (SourceNorthOfTarget(movingFrom, movingAcross))
            {
                //Move South
                oppositeCoordinates.Y++;
            }

            if (SourceSouthOfTarget(movingFrom, movingAcross))
            {
                //Move North
                oppositeCoordinates.Y--;
            }

            if (SourceEastOfTarget(movingFrom, movingAcross))
            {
                //Move West
                oppositeCoordinates.X--;
            }

            if (SourceWestOfTarget(movingFrom, movingAcross))
            {
                //Move East
                oppositeCoordinates.X++;
            }

            return oppositeCoordinates;
        }

        protected static bool CanAffordCommandCost(GameUnit commander, int commandCost)
        {
            return commander.IsCommander && commander.Stats.CurrentCmd >= commandCost;
        }
    }
}