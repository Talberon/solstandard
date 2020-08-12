using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses.Bard;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Bard
{
    public class SongAnthem : SongAction
    {
        private readonly int auraBonus;
        private readonly int selfBonus;
        private readonly int[] auraRange;

        public SongAnthem(int auraBonus, int selfBonus, int[] auraRange) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Anthem, GameDriver.CellSizeVector),
            name: "Song - Anthem",
            description:
            $"Increases {UnitStatistics.Abbreviation[Stats.Atk]} by [{auraBonus} Aura/{selfBonus} Solo] for units within the aura.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
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
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Performer must be playing first!", 50);
                AssetManager.WarningSFX.Play();
                return;
            }

            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                List<SongStatus> otherSongs = targetUnit.StatusEffects
                    .Where(status => status is SongStatus && !(status is AnthemStatus))
                    .Cast<SongStatus>().ToList();
                otherSongs.ForEach(song => targetUnit.StatusEffects.Remove(song));

                MapContainer.ClearDynamicAndPreviewGrids();
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(targetUnit, new AnthemStatus(auraBonus, selfBonus, auraRange))
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