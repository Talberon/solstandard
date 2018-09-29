using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Archer
{
    public class Draw : UnitSkill
    {
        private static readonly int[] Range = {0};
        private bool active;

        public Draw() : base(
            name: "Draw",
            description: "Increase your range by 1."
                         + "\nDecrease your range by 1 if this ability is already active.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action)
        )
        {
            active = false;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (targetSlice.UnitEntity == null || targetSlice.UnitEntity != GameContext.ActiveUnit.UnitEntity)
            {
                AssetManager.WarningSFX.Play();
                return;
            }

            if (!active)
            {
                for (int i = 0; i < GameContext.ActiveUnit.Stats.BaseAtkRange.Length; i++)
                {
                    GameContext.ActiveUnit.Stats.AtkRange[i]++;
                }
            }
            else
            {
                GameContext.ActiveUnit.Stats.AtkRange = GameContext.ActiveUnit.Stats.BaseAtkRange;
            }

            active = !active;
            AssetManager.SkillBuffSFX.Play();

            MapContainer.ClearDynamicGrid();
            mapContext.SelectedUnit.SetUnitAnimation(UnitSprite.UnitAnimationState.Idle);
            SkipCombatPhase(mapContext);
        }
    }
}