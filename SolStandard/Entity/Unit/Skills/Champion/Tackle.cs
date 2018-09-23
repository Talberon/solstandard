using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
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

        public override void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext)
        {
            if (target == null || target.GetType() != typeof(GameUnit)) return;

            if (target == GameContext.ActiveUnit)
            {
                //Skip the action if selecting self
                MapContainer.ClearDynamicGrid();
                AssetManager.MapUnitSelectSFX.Play();
                mapContext.ProceedToNextState();
                mapContext.ProceedToNextState();
                mapContext.SetPromptWindowText("Confirm End Turn");
            }
            else
            {
                GameUnit targetUnit = (GameUnit) target;
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.ShoveAction(targetUnit, mapContext))
                {
                    if (MoveToTarget(targetOriginalPosition))
                    {
                        GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);

                        if (BasicAttack.StartCombat(targetUnit, mapContext, battleContext))
                        {
                            mapContext.ProceedToNextState();
                            mapContext.SetPromptWindowText("Confirm End Turn");
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