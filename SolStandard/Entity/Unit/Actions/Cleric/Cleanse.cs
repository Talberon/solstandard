using System.Collections.Generic;
using System.Linq;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Cleric
{
    public class Cleanse : UnitAction
    {
        public Cleanse() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Cleanse, GameDriver.CellSizeVector),
            name: "Prayer - Cleanse",
            description: "Remove all cleansable status effects from target ally.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2},
            freeAction: false
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                if (TargetHasCleansableStatuses(targetUnit))
                {
                    CleanseAllCleansableStatuses(targetUnit);
                    GlobalEventQueue.QueueSingleEvent(new EndTurnEvent());
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("No cleansable status effects!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetHasCleansableStatuses(GameUnit targetUnit)
        {
            return targetUnit.StatusEffects.Any(status => status.CanCleanse);
        }

        public static void CleanseAllCleansableStatuses(GameUnit targetUnit)
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            var eventQueue = new Queue<IEvent>();

            foreach (StatusEffect effect in targetUnit.StatusEffects.Where(effect => effect.CanCleanse).ToList())
            {
                eventQueue.Enqueue(new RemoveStatusEffectEvent(targetUnit, effect));
                eventQueue.Enqueue(new WaitFramesEvent(50));
            }

            GlobalEventQueue.QueueEvents(eventQueue);
        }
    }
}