using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class CmdSonicStrike : LeapStrike, ICommandAction
    {
        private readonly int cmdCost;

        public CmdSonicStrike(int cmdCost) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Sonic Strike",
            description: "Leap towards an enemy to attack them; even across impassible terrain!" + Environment.NewLine +
                         "Select a target, then select a space to land on next to that target." + Environment.NewLine +
                         $"Free Action. Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
        }

        public override void CancelAction()
        {
            if (CurrentPhase == ActionPhase.SelectLandingSpace) GlobalContext.ActiveUnit.AddCommandPoints(cmdCost);
            base.CancelAction();
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CurrentPhase == ActionPhase.SelectTarget)
            {
                if (!CanAffordCommandCost(GlobalContext.ActiveUnit, cmdCost))
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                        $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                    AssetManager.WarningSFX.Play();
                    return;
                }

                GlobalContext.ActiveUnit.RemoveCommandPoints(cmdCost);
            }

            base.ExecuteAction(targetSlice);
        }
    }
}