using System;
using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Scenario;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Bard
{
    public class CmdSongBattleHymn : SongAction, ICommandAction
    {
        private readonly int cmdCost;
        private readonly int auraBonus;
        private readonly int selfBonus;
        private readonly int[] auraRange;

        public CmdSongBattleHymn(int cmdCost, int auraBonus, int selfBonus, int[] auraRange) : base(
            icon: ObjectiveIconProvider.GetObjectiveIcon(VictoryConditions.Seize, GameDriver.CellSizeVector),
            name: $"[{cmdCost}{UnitStatistics.Abbreviation[Stats.CommandPoints]}] Song - Battle Hymn",
            description:
            $"Applies a {UnitStatistics.Abbreviation[Stats.Atk]}/{UnitStatistics.Abbreviation[Stats.Retribution]} buff [{auraBonus} Aura/{selfBonus} Solo] for units within the aura." +
            Environment.NewLine + $"Costs {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
            this.cmdCost = cmdCost;
            this.auraBonus = auraBonus;
            this.selfBonus = selfBonus;
            this.auraRange = auraRange;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (!CanAffordCommandCost(GlobalContext.ActiveUnit, cmdCost))
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(
                    $"This action requires {cmdCost} {UnitStatistics.Abbreviation[Stats.CommandPoints]}!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            if (!SingerIsSinging)
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Performer must be playing first!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                GlobalContext.ActiveUnit.RemoveCommandPoints(cmdCost);

                List<SongStatus> otherSongs = targetUnit.StatusEffects
                    .Where(status => status is SongStatus && !(status is TempestStatus))
                    .Cast<SongStatus>().ToList();
                otherSongs.ForEach(song => targetUnit.StatusEffects.Remove(song));

                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(targetUnit, new BattleHymnStatus(auraBonus, selfBonus, auraRange))
                );
                eventQueue.Enqueue(new WaitFramesEvent(30));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}