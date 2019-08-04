using System;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class CmdAlchemicalArmor : UnitAction
    {
        private readonly int cmdCost;
        private readonly int blk;
        private readonly int hpPerTurn;
        private readonly int amrPerTurn;
        private readonly int duration;

        public CmdAlchemicalArmor(int cmdCost, int blk, int hpPerTurn, int amrPerTurn, int duration) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"Alchemical Armor [{cmdCost}]",
            description:
            $"Cast a status effect on self that grants {blk} {UnitStatistics.Abbreviation[Stats.Block]}, " +
            $"{hpPerTurn} {UnitStatistics.Abbreviation[Stats.Hp]} per turn, " +
            $"and {amrPerTurn} {UnitStatistics.Abbreviation[Stats.Armor]} per turn for [{duration}] turns."
            + Environment.NewLine +
            $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            this.blk = blk;
            this.hpPerTurn = hpPerTurn;
            this.amrPerTurn = amrPerTurn;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!CanAffordCommandCost(GameContext.ActiveUnit, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                AssetManager.SkillBuffSFX.Play();
                AssetManager.MenuConfirmSFX.Play();
                GlobalEventQueue.QueueSingleEvent(
                    new CastStatusEffectEvent(targetUnit, new AlchemicalArmorStatus(blk, hpPerTurn, amrPerTurn, duration))
                );
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}