using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Bard
{
    public class SongTempest : SongAction
    {
        private readonly int auraBonus;
        private readonly int selfBonus;
        private readonly int[] auraRange;

        public SongTempest(int auraBonus, int selfBonus, int[] auraRange) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.DoubleTime, GameDriver.CellSizeVector),
            name: "Tempest",
            description:
            $"Applies a {UnitStatistics.Abbreviation[Stats.Mv]} buff [{auraBonus} Aura/{selfBonus} Solo] for units within the aura at the start of the turn.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: auraRange,
            freeAction: true
        )
        {
            this.auraBonus = auraBonus;
            this.selfBonus = selfBonus;
            this.auraRange = auraRange;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (!SingerIsSinging)
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Singer must be singing!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                List<SongStatus> otherSongs = targetUnit.StatusEffects
                    .Where(status => status is SongStatus && !(status is TempestStatus))
                    .Cast<SongStatus>().ToList();
                otherSongs.ForEach(song => targetUnit.StatusEffects.Remove(song));

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                GlobalEventQueue.QueueSingleEvent(
                    new CastStatusEffectEvent(targetUnit, new TempestStatus(auraBonus, selfBonus, auraRange))
                );
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}