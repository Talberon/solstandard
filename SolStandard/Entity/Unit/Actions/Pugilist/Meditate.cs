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

namespace SolStandard.Entity.Unit.Actions.Pugilist
{
    public class Meditate : UnitAction
    {
        public Meditate() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Meditate, new Vector2(GameDriver.CellSize)),
            name: "Meditate",
            description: "Remove all cleansable status effects on self.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
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
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Must target self!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}