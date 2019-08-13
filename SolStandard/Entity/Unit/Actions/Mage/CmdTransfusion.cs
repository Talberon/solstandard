using System;
using System.Collections.Generic;
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
    public class CmdTransfusion : UnitAction, ICommandAction
    {
        private readonly int cmdCost;

        public CmdTransfusion(int cmdCost) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Sanguimancy - Transfusion",
            description:
            $"Perform a basic attack against an enemy and regenerate {UnitStatistics.Abbreviation[Stats.Hp]} equal to the damage dealt." +
            Environment.NewLine + $"Does not regenerate {UnitStatistics.Abbreviation[Stats.Armor]}." +
            Environment.NewLine + $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1, 2},
            freeAction: false
        )
        {
            this.cmdCost = cmdCost;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (!CanAffordCommandCost(GameContext.ActiveUnit, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);
            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                GameContext.ActiveUnit.RemoveCommandPoints(cmdCost);

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(GameContext.ActiveUnit, new DamageToHealthStatus(Icon, 1))
                );
                eventQueue.Enqueue(new StartCombatEvent(targetUnit));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Can't attack here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}