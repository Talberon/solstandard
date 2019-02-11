using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public abstract class DamageOverTimeAction : UnitAction
    {
        private readonly int damagePerTurn;
        private readonly int duration;
        private readonly string toastMessage;

        protected DamageOverTimeAction(SkillIcon icon, string name, int duration, int damagePerTurn, int[] range,
            string toastMessage) :
            base(
                icon: SkillIconProvider.GetSkillIcon(icon, new Vector2(GameDriver.CellSize)),
                name: name,
                description: "Deal [" + damagePerTurn + "] damage at the beginning of target's turn for [" + duration +
                             "] turns.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
                range: range
            )
        {
            this.damagePerTurn = damagePerTurn;
            this.toastMessage = toastMessage;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new CastStatusEffectEvent(targetUnit,
                        new DamageOverTimeStatus(Icon, duration, damagePerTurn, toastMessage, Name))
                );
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not an enemy in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}