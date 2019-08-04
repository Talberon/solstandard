using System;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class CmdSonicStrike : LeapStrike
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
            GameContext.ActiveUnit.AddCommandPoints(cmdCost);
            base.CancelAction();
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CurrentPhase == ActionPhase.SelectTarget)
            {
                if (!CanAffordCommandCost(GameContext.ActiveUnit, cmdCost))
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                        $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                    AssetManager.WarningSFX.Play();
                    return;
                }

                GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
            }

            base.ExecuteAction(targetSlice);
        }
    }
}