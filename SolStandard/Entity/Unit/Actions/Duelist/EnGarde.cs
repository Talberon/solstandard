using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class EnGarde : UnitAction
    {
        private readonly int blockBonus;

        public EnGarde(int blockBonus) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.EnGarde, GameDriver.CellSizeVector),
            name: "En Garde",
            description:
            $"Attack a target while applying a temporary [+{blockBonus} {UnitStatistics.Abbreviation[Stats.Block]}] bonus to self.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1},
            freeAction: false
        )
        {
            this.blockBonus = blockBonus;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(new CastStatusEffectEvent(
                    GlobalContext.ActiveUnit,
                    new TempCombatStatus(new BlkStatUp(0, blockBonus))
                ));
                GlobalEventQueue.QueueSingleEvent(new StartCombatEvent(targetUnit));
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}