using System;
using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Cleric
{
    public class CmdGodsbreath : UnitAction, ICommandAction
    {
        private readonly int cmdCost;
        private readonly int amountToHeal;
        private readonly int buffTurnDuration;

        public CmdGodsbreath(int cmdCost, int amountToHeal, int buffTurnDuration) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Godsbreath",
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
            GameUnit actor = GlobalContext.ActiveUnit;
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!CanAffordCommandCost(actor, cmdCost))
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (TargetIsSelfInRange(targetSlice, targetUnit) || TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                actor.RemoveCommandPoints(cmdCost);

                var alliesInRange = new List<GameUnit>();

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

                var events = new Queue<IEvent>();
                foreach (GameUnit ally in alliesInRange)
                {
                    var hpRegen = new HealthRegeneration(buffTurnDuration, amountToHeal);
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
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self or ally!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}