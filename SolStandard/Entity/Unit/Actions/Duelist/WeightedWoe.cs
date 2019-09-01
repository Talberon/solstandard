using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class WeightedWoe : UnitAction
    {
        public WeightedWoe() : base(
            //TODO New Icon
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, GameDriver.CellSizeVector),
            name: "Weighted Woe",
            description:
            $"Attack an enemy unit, dealing damage equal to their missing {UnitStatistics.Abbreviation[Stats.Armor]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                int targetMissingArmor = targetUnit.Stats.MaxArmor - targetUnit.Stats.CurrentArmor;
                WeaponStatistics antiArmorWeapon = new WeaponStatistics(targetMissingArmor, 0, Range, 1);
                GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(
                    targetUnit,
                    false,
                    GameContext.ActiveUnit.Stats.ApplyWeaponStatistics(antiArmorWeapon, true)
                ));
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}