using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Skills.Champion
{
    public class Tackle : UnitSkill
    {
        public Tackle() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Tackle, new Vector2(32)),
            name: "Tackle",
            description: "Shove an enemy if there is an empty space behind them,"
                         + "\nthen follow up by moving into their space and attacking.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.ShoveAction(targetUnit))
                {
                    MoveToTarget(targetOriginalPosition);

                    GenerateActionGrid(GameContext.ActiveUnit.UnitEntity.MapCoordinates);
                    BasicAttack.StartCombat(targetUnit, mapContext, battleContext);

                    EnterCombatPhase(mapContext);
                }
                else
                {
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private static void MoveToTarget(Vector2 targetCoordinates)
        {
            GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetCoordinates;
        }
    }
}