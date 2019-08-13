using System;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Marauder
{
    public class CmdBerserk : UnitAction, IIncrementableAction, ICommandAction
    {
        private const string SkillName = "Berserk";

        private readonly int cmdCost;
        public int Value { get; private set; }

        public CmdBerserk(int cmdCost) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] {SkillName}: 0 {UnitStatistics.Abbreviation[Stats.Hp]}",
            description: GenerateActionDescription(cmdCost),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            Value = 0;
        }

        private static WindowContentGrid GenerateActionDescription(int cmdCost)
        {
            Vector2 iconSize = GameDriver.CellSizeVector;

            return new WindowContentGrid(new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,
                            "As a free action, deal X damage to self. Cannot deal fatal damage." + Environment.NewLine +
                            $"Costs {cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}."),
                        new RenderBlank(),
                        new RenderBlank(),
                        new RenderBlank(),
                        new RenderBlank()
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Adjust damage value with "),
                        InputIconProvider.GetInputIcon(Input.TabLeft, iconSize),
                        new RenderText(AssetManager.WindowFont, " and "),
                        InputIconProvider.GetInputIcon(Input.TabRight, iconSize),
                        new RenderText(AssetManager.WindowFont, ".")
                    }
                },
                2
            );
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

            if (Value < 1)
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor(
                    "Must specify at least 1 point of damage!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                actor.RemoveCommandPoints(cmdCost);

                for (int i = 0; i < Value; i++)
                {
                    actor.DamageUnit();
                }

                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor($"Dealt {Value} damage to self!", 50);
                AssetManager.CombatDamageSFX.Play();

                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(50));
                GlobalEventQueue.QueueSingleEvent(new AdditionalActionEvent());
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public void Increment(int amountToIncrement)
        {
            int maxDamage = GameContext.ActiveUnit.Stats.CurrentHP - 1;

            if (Value + amountToIncrement > maxDamage)
            {
                Value = maxDamage;
            }
            else
            {
                Value += amountToIncrement;
            }

            UpdateSkillName();
            AssetManager.MenuMoveSFX.Play();
        }

        public void Decrement(int amountToDecrement)
        {
            if (Value - amountToDecrement < 0)
            {
                Value = 0;
            }
            else
            {
                Value -= amountToDecrement;
            }

            UpdateSkillName();
            AssetManager.MenuMoveSFX.Play();
        }

        private void UpdateSkillName()
        {
            Name = $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] {SkillName}: {Value} {UnitStatistics.Abbreviation[Stats.Hp]}";
            GameContext.GameMapContext.RefreshCurrentActionMenuOption();
        }
    }
}