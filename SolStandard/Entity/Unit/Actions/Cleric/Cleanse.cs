using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
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
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Cleanse, new Vector2(GameDriver.CellSize)),
            name: "Cleanse",
            description: "Remove all cleansable status effects from target ally.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit))
            {
                CleanseAllCleansableStatuses(targetUnit);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an ally in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void CleanseAllCleansableStatuses(GameUnit targetUnit)
        {
            MapContainer.ClearDynamicAndPreviewGrids();

            Queue<IEvent> eventQueue = new Queue<IEvent>();

            foreach (StatusEffect effect in targetUnit.StatusEffects.Where(effect => effect.CanCleanse).ToList())
            {
                eventQueue.Enqueue(new RemoveStatusEffectEvent(targetUnit, effect));
                eventQueue.Enqueue(new WaitFramesEvent(50));
            }

            eventQueue.Enqueue(new EndTurnEvent());
            GlobalEventQueue.QueueEvents(eventQueue);
        }
    }
}