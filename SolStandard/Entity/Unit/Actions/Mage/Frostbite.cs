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

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Frostbite : UnitAction
    {
        private readonly int mvToReduce;
        private readonly int duration;

        public Frostbite(int duration, int mvToReduce) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Immobilize, new Vector2(GameDriver.CellSize)),
            name: "Cryomancy - Frostbite",
            description: "Reduce target's " + UnitStatistics.Abbreviation[Stats.Mv] +
                         " stat by [" + mvToReduce + "] for [" + duration + "] turn(s).",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.mvToReduce = mvToReduce;
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
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new MoveStatDown(statusDuration, mvToReduce)));
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