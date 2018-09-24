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

        public Tackle(string name, SpriteAtlas tileSprite) : base(name, tileSprite)
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (targetUnit == null || targetUnit.GetType() != typeof(GameUnit)) return;

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