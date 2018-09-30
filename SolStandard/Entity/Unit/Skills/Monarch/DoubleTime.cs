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
        private readonly int statModifier;
        private readonly int duration;

        public DoubleTime(int duration, int statModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.DoubleTime, new Vector2(32)),
            name: "Double Time",
            description: "Grant a buff that increases an ally's MV by [+" + statModifier + "] for [" + duration +
                         "] turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.statModifier = statModifier;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                AssetManager.SkillBuffSFX.Play();
                targetUnit.AddStatusEffect(new MoveStatUp(duration, statModifier));
                MapContainer.ClearDynamicAndPreviewGrids();
                SkipCombatPhase(mapContext);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}