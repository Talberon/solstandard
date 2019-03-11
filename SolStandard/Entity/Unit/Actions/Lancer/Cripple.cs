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

namespace SolStandard.Entity.Unit.Actions.Lancer
{
    public class Cripple : UnitAction
    {
        private readonly int statModifier;
        private readonly int duration;

        public Cripple(int duration, int statModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Immobilize, new Vector2(GameDriver.CellSize)),
            name: "Cripple",
            description: "Reduce target's " + UnitStatistics.Abbreviation[Stats.Mv] +
                         " stat by [" + statModifier + "] for [" + duration + "] turn(s).",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.statModifier = statModifier;
            this.duration = duration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();
                
                int statusDuration = (targetUnit.IsExhausted) ? duration + 1 : duration;

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new MoveStatModifier(statusDuration, statModifier)));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}