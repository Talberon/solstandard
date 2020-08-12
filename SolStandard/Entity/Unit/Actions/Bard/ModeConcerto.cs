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
    public class ModeConcerto : UnitAction
    {
        public const string GroupSkillName = "Concerto";

        public ModeConcerto() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Concerto, GameDriver.CellSizeVector),
            name: "Play - " + GroupSkillName,
            description:
            "Applies song effects to allies in range with reduced potency." + Environment.NewLine +
            $"Removes {ModeSolo.SoloSkillName}.",
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

                foreach (SoloStatus concerto in targetUnit.StatusEffects.Where(status => status is SoloStatus)
                    .Cast<SoloStatus>())
                {
                    concerto.RemoveEffect(targetUnit);
                }

                targetUnit.StatusEffects.RemoveAll(status => status is SoloStatus);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new ConcertoStatus()));
                eventQueue.Enqueue(new ToastAtCursorEvent("Song range extended at reduced potency!"));
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