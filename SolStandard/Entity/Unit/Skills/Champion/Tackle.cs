using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Champion
{
    public class Tackle : UnitSkill
    {
        private static readonly int[] Range = {1};

        public Tackle(SpriteAtlas tileSprite) : base(tileSprite)
        {
            Name = "Tackle";
            Description = "Shove an enemy if there is an empty space behind them,"
                          + "\nthen follow up by moving into their space and attacking.";
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit == null || targetUnit.GetType() != typeof(GameUnit))
            {
                AssetManager.WarningSFX.Play();
                return;
            }

            if (targetUnit == GameContext.ActiveUnit)
            {
                //Skip the action if selecting self
                MapContainer.ClearDynamicGrid();
                AssetManager.MapUnitSelectSFX.Play();
                SkipCombatPhase(mapContext);
            }
            else
            {
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.ShoveAction(targetUnit, mapContext))
                {
                    GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);

                    if (MoveToTarget(targetOriginalPosition))
                    {
                        GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);

                        if (BasicAttack.StartCombat(targetUnit, mapContext, battleContext))
                        {
                            EnterCombatPhase(mapContext);
                        }
                    }
                }
            }
        }

        private static bool MoveToTarget(Vector2 targetCoordinates)
        {
            if (UnitMovingContext.CanMoveAtCoordinates(targetCoordinates))
            {
                GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetCoordinates;
                return true;
            }

            return false;
        }
    }
}