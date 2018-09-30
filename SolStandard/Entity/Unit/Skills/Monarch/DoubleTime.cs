using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Monarch
{
    public class DoubleTime : UnitSkill
    {
        private static readonly int[] Range = {1};
        private readonly int statModifier;
        private readonly int duration;

        public DoubleTime(int duration, int statModifier) : base(
            name: "Double Time",
            description: "Grant a buff that increases an ally's MV by " + statModifier + " for " + duration + " turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action)
        )
        {
            this.statModifier = statModifier;
            this.duration = duration;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            UnitTargetingContext unitTargetingContext = new UnitTargetingContext(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (targetUnit == null || targetUnit == GameContext.ActiveUnit || targetSlice.DynamicEntity == null)
            {
                AssetManager.WarningSFX.Play();
            }
            else if (targetUnit.Team == GameContext.ActiveUnit.Team)
            {
                AssetManager.SkillBuffSFX.Play();
                targetUnit.AddStatusEffect(new MoveStatUp(duration, statModifier));
                MapContainer.ClearDynamicGrid();
                SkipCombatPhase(mapContext);
            }
        }
    }
}