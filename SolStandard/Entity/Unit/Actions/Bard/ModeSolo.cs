using System;
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
    public class ModeSolo : UnitAction
    {
        public const string SoloSkillName = "Solo";

        public ModeSolo() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Solo, GameDriver.CellSizeVector),
            name: "Play - " + SoloSkillName,
            description:
            "Applies song effects to self with increased potency." + Environment.NewLine +
            $"Removes {ModeConcerto.GroupSkillName}.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: true
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();


                foreach (ConcertoStatus concerto in targetUnit.StatusEffects.Where(status => status is ConcertoStatus)
                    .Cast<ConcertoStatus>())
                {
                    concerto.RemoveEffect(targetUnit);
                }

                targetUnit.StatusEffects.RemoveAll(status => status is ConcertoStatus);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new SoloStatus()));
                eventQueue.Enqueue(new ToastAtCursorEvent("Song range limited at increased potency!"));
                eventQueue.Enqueue(new WaitFramesEvent(50));
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