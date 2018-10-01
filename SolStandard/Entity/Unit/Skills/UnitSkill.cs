using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public abstract class UnitSkill
    {
        public IRenderable Icon { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        protected readonly SpriteAtlas TileSprite;
        public int[] Range { get; private set; }

        protected UnitSkill(IRenderable icon, string name, string description, SpriteAtlas tileSprite, int[] range)
        {
            Icon = icon;
            Name = name;
            Description = description;
            TileSprite = tileSprite;
            Range = range;
        }

        public virtual void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateRealTargetingGrid(origin, Range);
        }

        public abstract void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext);

        // ReSharper disable once MemberCanBeMadeStatic.Global
        public void CancelAction(MapContext mapContext)
        {
            MapContainer.ClearDynamicAndPreviewGrids();
            mapContext.RevertToPreviousState();
            AssetManager.MapUnitCancelSFX.Play();
        }

        protected static void EnterEndPhase(MapContext mapContext)
        {
            mapContext.CurrentTurnState = MapContext.TurnState.ResolvingTurn;
            mapContext.SetPromptWindowText("Confirm End Turn");
        }

        protected static void EnterActionPhase(MapContext mapContext)
        {
            mapContext.CurrentTurnState = MapContext.TurnState.UnitActing;
            mapContext.SetPromptWindowText("Confirm End Turn");
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
                   targetSlice.TerrainEntity.GetType() == typeof(BreakableObstacle);
        }
    }
}