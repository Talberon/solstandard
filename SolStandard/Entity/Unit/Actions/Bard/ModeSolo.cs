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
    public class ModeSolo : UnitAction
    {
        public const string SoloSkillName = "Solo";

        public ModeSolo() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.BasicAttack, GameDriver.CellSizeVector),
            name: SoloSkillName,
            description:
            $"Applies status effects from songs to self with an increased potency. Removes {ModeConcerto.GroupSkillName}.",
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

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                GlobalEventQueue.QueueSingleEvent(new CastStatusEffectEvent(targetUnit, new SoloStatus()));
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