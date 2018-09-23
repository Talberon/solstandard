using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class BasicAttack : UnitSkill
    {
        private readonly int[] range;

        public BasicAttack(string name, SpriteAtlas tileSprite, int[] range) : base(name, tileSprite)
        {
            this.range = range;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, range);
        }

        public override void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext)
        {
            if (target != null && target.GetType() == typeof(GameUnit))
            {
                //Skip the combat state if player selects the same unit
                if (mapContext.SelectedUnit == target)
                {
                    MapContainer.ClearDynamicGrid();
                    mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                    mapContext.ProceedToNextState();
                    mapContext.ProceedToNextState();
                    mapContext.SetPromptWindowText("Confirm End Turn");
                    AssetManager.MapUnitSelectSFX.Play();
                }
                else
                {
                    if (StartCombat((GameUnit) target, mapContext, battleContext))
                    {
                        mapContext.ProceedToNextState();
                        mapContext.SetPromptWindowText("Confirm End Turn");
                    }
                }
            }
        }

        public static bool StartCombat(GameUnit target, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit attackingUnit = mapContext.SelectedUnit;
            GameUnit defendingUnit = target;

            if (mapContext.TargetUnitIsLegal(defendingUnit))
            {
                MapContainer.ClearDynamicGrid();
                battleContext.StartNewCombat(attackingUnit,
                    MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                    defendingUnit,
                    MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates));

                AssetManager.CombatStartSFX.Play();
                return true;
            }

            AssetManager.WarningSFX.Play();
            return false;
        }
    }
}