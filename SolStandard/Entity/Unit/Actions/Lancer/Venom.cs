using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class Venom : UnitAction
    {
        private readonly int retDebuffValue;
        private readonly int duration;

        public Venom(int retDebuffValue, int duration) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Venom, GameDriver.CellSizeVector),
            name: "Venom",
            description: "Reduce a target's " + UnitStatistics.Abbreviation[Stats.Retribution] + " stat by " +
                         retDebuffValue + " for " + duration + " turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: true
        )
        {
            this.retDebuffValue = retDebuffValue;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GlobalEventQueue.QueueSingleEvent(new CastStatusEffectEvent(targetUnit,
                    new RetributionStatDown(duration, retDebuffValue)));
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}