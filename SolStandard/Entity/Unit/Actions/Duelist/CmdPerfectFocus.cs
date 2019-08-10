using System;
using System.Linq;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Entity.Unit.Statuses.Duelist;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Duelist
{
    public class CmdPerfectFocus : UnitAction
    {
        private readonly int cmdCost;
        private readonly int maxActions;

        public CmdPerfectFocus(int cmdCost, int maxActions) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Perfect Focus",
            description: $"Max out focus points [{maxActions} pts] as a free action. " + Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            this.maxActions = maxActions;
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
                if (targetUnit.StatusEffects.SingleOrDefault(status => status is FocusStatus) is FocusStatus
                    currentFocus)
                {
                    if (currentFocus.FocusPoints < maxActions)
                    {
                        GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                        AssetManager.SkillBuffSFX.Play();
                        AssetManager.MenuConfirmSFX.Play();
                        GlobalEventQueue.QueueSingleEvent(
                            new CastStatusEffectEvent(targetUnit, new FocusStatus(maxActions, true))
                        );
                        GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                    }
                    else
                    {
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Max focus points reached!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);
                    GlobalEventQueue.QueueSingleEvent(
                        new CastStatusEffectEvent(
                            targetUnit,
                            new FocusStatus(maxActions, true)
                        )
                    );
                    GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                    GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}