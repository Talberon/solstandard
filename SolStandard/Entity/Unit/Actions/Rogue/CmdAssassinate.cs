using System;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Rogue
{
    public class CmdAssassinate : UnitAction, ICommandAction
    {
        private readonly int cmdCost;
        private readonly int damageThreshold;

        public CmdAssassinate(int cmdCost, int damageThreshold) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Assassinate",
            description:
            $"Immediately destroy an enemy that has [{damageThreshold}] {UnitStatistics.Abbreviation[Stats.Hp]} or fewer." +
            Environment.NewLine + $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.cmdCost = cmdCost;
            this.damageThreshold = damageThreshold;
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

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                int targetCurrentHP = targetUnit.Stats.CurrentHP;
                if (targetCurrentHP <= damageThreshold)
                {
                    GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);

                    for (int i = 0; i < targetCurrentHP; i++)
                    {
                        targetUnit.DamageUnit(true);
                    }

                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                        $"Assassinated {targetUnit.Id}!", 50
                    );

                    GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                    GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                        $"Target must have [{damageThreshold}] {UnitStatistics.Abbreviation[Stats.Hp]} or fewer!", 50
                    );
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}