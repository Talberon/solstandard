using System;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Paladin
{
    public class CmdSteelheart : UnitAction, ICommandAction
    {
        private readonly int cmdCost;

        public CmdSteelheart(int cmdCost) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Steelheart",
            description: $"As a free action, deplete all {UnitStatistics.Abbreviation[Stats.Armor]} " +
                         $"and recover {UnitStatistics.Abbreviation[Stats.Hp]} equal to the " +
                         $"{UnitStatistics.Abbreviation[Stats.Armor]} lost." + Environment.NewLine +
                         $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit actor = GameContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!CanAffordCommandCost(actor, cmdCost))
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                if (actor.Stats.CurrentHP == actor.Stats.MaxHP)
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor($"{UnitStatistics.Abbreviation[Stats.Hp]} is already maxed out!", 50);
                    AssetManager.WarningSFX.Play();
                    return;
                }

                if (actor.Stats.CurrentArmor == 0)
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor($"Must have {UnitStatistics.Abbreviation[Stats.Armor]} to spend!", 50);
                    AssetManager.WarningSFX.Play();
                    return;
                }
                
                actor.RemoveCommandPoints(cmdCost);
                AssetManager.SkillBuffSFX.Play();
                AssetManager.MenuConfirmSFX.Play();

                int actorAmr = actor.Stats.CurrentArmor;

                for (int i = 0; i < actorAmr; i++)
                {
                    actor.DamageUnit();
                }

                actor.RecoverHP(actorAmr);

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    $"Recovered {actorAmr} {UnitStatistics.Abbreviation[Stats.Hp]} for {actorAmr} {UnitStatistics.Abbreviation[Stats.Armor]}!",
                    50);

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