using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Monarch
{
    public class DoubleTime : UnitSkill
    {
        private static readonly int[] Range = {1};

        public DoubleTime(SpriteAtlas tileSprite) : base(tileSprite)
        {
            Name = "Double Time";
            Description = "Grant a buff of +1 Mv to an Ally in exchange for -1 Def."
                          + "\nCan only use if target's Def is greater than 1.";
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (targetUnit == null || targetUnit == GameContext.ActiveUnit || targetUnit.Stats.Def <= 1)
            {
                AssetManager.WarningSFX.Play();
            }
            else if (targetUnit.Team == GameContext.ActiveUnit.Team)
            {
                AssetManager.SkillBuffSFX.Play();
                targetUnit.Stats.Def--;
                targetUnit.Stats.Mv++;
                MapContainer.ClearDynamicGrid();
                SkipCombatPhase(mapContext);
            }
        }
    }
}