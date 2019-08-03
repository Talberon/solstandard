using System;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.Unit.Actions.Champion
{
    public class CmdWarmaster : UnitAction
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
            name: $"Warmaster <{cmdCost}>",
            description: $"Force another unit to move up to {maxDistance} spaces. " + Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: false
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
                        GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
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
            GameUnit actor = GameContext.ActiveUnit;

            if (!CanAffordCommandCost(actor, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            if (TargetIsUnitInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                new Sprint(maxDistance).GenerateActionGrid(targetUnit.UnitEntity.MapCoordinates);
                return true;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }

        private bool MoveTarget(MapSlice targetSlice)
        {
            if (Sprint.CanMove(GameContext.ActiveUnit))
            {
                if (CanMoveToTargetTile(targetSlice))
                {
                    Sprint.MoveUnitToTargetPosition(targetUnit, targetSlice.MapCoordinates);
                    return true;
                }

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a valid tile!", 50);
                AssetManager.WarningSFX.Play();
                return false;
            }

            GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't move!", 50);
            AssetManager.WarningSFX.Play();
            return false;
        }
    }
}