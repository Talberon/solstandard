using System;
using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Cleric
{
    public class CmdHealingAura : UnitAction
    {
        private readonly int cmdCost;
        private readonly int amountToHeal;
        private readonly int buffTurnDuration;

        public CmdHealingAura(int cmdCost, int amountToHeal, int buffTurnDuration) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Healing Aura",
            description:
            $"Allies in range gain a status that recovers {amountToHeal} {UnitStatistics.Abbreviation[Stats.Hp]} for {buffTurnDuration} turns." +
            Environment.NewLine +
            $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0, 1, 2},
            freeAction: false
        )
        {
            this.cmdCost = cmdCost;
            this.amountToHeal = amountToHeal;
            this.buffTurnDuration = buffTurnDuration;
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

            if (TargetIsSelfInRange(targetSlice, targetUnit) || TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                actor.RemoveCommandPoints(cmdCost);

                List<GameUnit> alliesInRange = new List<GameUnit>();

                List<MapElement> tilesInRange = MapContainer.GetMapElementsFromLayer(Layer.Dynamic);
                foreach (MapElement tileInRange in tilesInRange)
                {
                    MapSlice slice = MapContainer.GetMapSliceAtCoordinates(tileInRange.MapCoordinates);
                    GameUnit unitInRange = UnitSelector.SelectUnit(slice.UnitEntity);
                    if (unitInRange != null && unitInRange.Team == actor.Team)
                    {
                        alliesInRange.Add(unitInRange);
                    }
                }

                Queue<IEvent> events = new Queue<IEvent>();
                foreach (GameUnit ally in alliesInRange)
                {
                    HealthRegeneration hpRegen = new HealthRegeneration(buffTurnDuration, amountToHeal);
                    events.Enqueue(new CastStatusEffectEvent(ally, hpRegen));
                    events.Enqueue(new ToastAtCoordinatesEvent(ally.UnitEntity.MapCoordinates, hpRegen.Name,
                        AssetManager.SkillBuffSFX));
                    events.Enqueue(new WaitFramesEvent(10));
                }

                GlobalEventQueue.QueueEvents(events);
                GlobalEventQueue.QueueSingleEvent(new WaitFramesEvent(30));
                GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());

                MapContainer.ClearDynamicAndPreviewGrids();
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self or ally!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}