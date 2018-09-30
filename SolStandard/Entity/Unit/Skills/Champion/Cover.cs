using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Champion
{
    public class Cover : UnitSkill
    {
        private readonly int statModifier;
        private readonly int duration;

        public Cover(int duration, int statModifier) : base(
            name: "Cover",
            description: "Grant a buff that increases an ally's DEF by [+" + statModifier + "] for [" + duration +
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

            if (targetUnit == null || targetUnit == GameContext.ActiveUnit || targetSlice.DynamicEntity == null)
            {
                AssetManager.WarningSFX.Play();
            }
            else if (targetUnit.Team == GameContext.ActiveUnit.Team)
            {
                AssetManager.SkillBuffSFX.Play();
                targetUnit.AddStatusEffect(new DefStatUp(duration, statModifier));
                MapContainer.ClearDynamicGrid();
                SkipCombatPhase(mapContext);
            }
        }
    }
}