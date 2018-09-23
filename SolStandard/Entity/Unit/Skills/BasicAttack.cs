using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills
{
    public class BasicAttack : UnitSkill
    {
        public BasicAttack(string name, SpriteAtlas tileSprite, int[] range) : base(name, tileSprite, range)
        {
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(GameEntity target, MapContext mapContext, BattleContext battleContext)
        {
            StartCombat(mapContext, battleContext);
        }

        private void StartCombat(MapContext mapContext, BattleContext battleContext)
        {
            GameUnit attackingUnit = mapContext.SelectedUnit;
            GameUnit defendingUnit = UnitSelector.SelectUnit(MapContainer.GetMapSliceAtCursor().UnitEntity);

            if (mapContext.TargetUnitIsLegal(defendingUnit))
            {
                MapContainer.ClearDynamicGrid();
                battleContext.StartNewCombat(attackingUnit,
                    MapContainer.GetMapSliceAtCoordinates(attackingUnit.UnitEntity.MapCoordinates),
                    defendingUnit,
                    MapContainer.GetMapSliceAtCoordinates(defendingUnit.UnitEntity.MapCoordinates));

                mapContext.SetPromptWindowText("Confirm End Turn");
                mapContext.ProceedToNextState();

                AssetManager.CombatStartSFX.Play();
            }
            //Skip the combat state if player selects the same unit
            else if (attackingUnit == defendingUnit)
            {
                MapContainer.ClearDynamicGrid();
                mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
                mapContext.ProceedToNextState();
                mapContext.SetPromptWindowText("Confirm End Turn");
                mapContext.ProceedToNextState();

                AssetManager.MapUnitSelectSFX.Play();
            }
        }
    }
}