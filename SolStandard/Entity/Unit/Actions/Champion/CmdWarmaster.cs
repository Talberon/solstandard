using System;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class CmdWarmaster : UnitAction, ICommandAction
    {
        private readonly int cmdCost;
        private readonly int maxDistance;

        private enum ActionPhase
        {
            SelectTarget,
            MoveTarget
        }

        private ActionPhase currentPhase = ActionPhase.SelectTarget;
        private GameUnit targetUnit;

        public CmdWarmaster(int cmdCost, int maxDistance, int[] range) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Warmaster",
            description: $"Force another unit to move up to {maxDistance} spaces. " + Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            this.maxDistance = maxDistance;
        }

        public override void CancelAction()
        {
            currentPhase = ActionPhase.SelectTarget;
            base.CancelAction();
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            switch (currentPhase)
            {
                case ActionPhase.SelectTarget:
                    if (SelectTarget(targetSlice)) currentPhase = ActionPhase.MoveTarget;
                    break;
                case ActionPhase.MoveTarget:
                    if (MoveTarget(targetSlice))
                    {
                        GlobalContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                        currentPhase = ActionPhase.SelectTarget;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SelectTarget(MapSlice targetSlice)
        {
            targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            GameUnit actor = GlobalContext.ActiveUnit;

            if (!CanAffordCommandCost(actor, cmdCost))
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                Sprint.GenerateSprintGrid(targetUnit.UnitEntity.MapCoordinates, targetUnit, maxDistance);
                AssetManager.MapUnitSelectSFX.Play();
                return true;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private bool MoveTarget(MapSlice targetSlice)
        {
            if (Sprint.CanMove(GlobalContext.ActiveUnit))
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    Sprint.MoveUnitToTargetPosition(targetUnit, targetSlice.MapCoordinates);
                    GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    return true;
                }

                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a valid tile!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't move!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }
    }
}