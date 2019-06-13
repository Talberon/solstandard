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
    public class Atrophy : UnitAction
    {
        private readonly int statModifier;
        private readonly int duration;

        public Atrophy(int duration, int statModifier) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Atrophy, new Vector2(GameDriver.CellSize)),
            name: "Atrophy",
            description: "Reduce an enemy's " + UnitStatistics.Abbreviation[Stats.Atk] + " by [" + statModifier + "]" +
                         " for [" + duration + "] turns.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1, 2, 3},
            freeAction: false
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

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit, new AtkStatDown(duration, statModifier)));
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