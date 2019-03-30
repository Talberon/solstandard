using System;
using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Item
{
    public class ConsumeBuffItemAction : UnitAction
    {
        private readonly IConsumable item;
        private readonly Stats statistic;
        private readonly int statModifier;
        private readonly int buffDuration;

        public ConsumeBuffItemAction(IConsumable item, Stats statistic, int statModifier, int buffDuration,
            int[] range) : base(
            icon: item.Icon,
            name: "Consume: " + item.Name,
            description: string.Format("Single use. Target modifies {0} by [{1}] for [{2}] turns.",
                UnitStatistics.Abbreviation[statistic], statModifier, buffDuration),
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: range,
            freeAction: true
        )
        {
            this.item = item;
            this.statistic = statistic;
            this.statModifier = statModifier;
            this.buffDuration = buffDuration;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnAllyInRange(targetSlice, targetUnit) || TargetIsSelfInRange(targetSlice, targetUnit))
            {
                item.Consume();
                
                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();

                switch (statistic)
                {
                    case Stats.Atk:
                        eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                            new AtkStatModifier(buffDuration, statModifier)));
                        break;
                    case Stats.Mv:
                        eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                            new MoveStatModifier(buffDuration, statModifier)));
                        break;
                    case Stats.AtkRange:
                        eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                            new AtkRangeStatUp(buffDuration, statModifier)));
                        break;
                    case Stats.Luck:
                        eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                            new LuckStatUp(buffDuration, statModifier)));
                        break;
                    case Stats.Retribution:
                        eventQueue.Enqueue(new CastStatusEffectEvent(targetUnit,
                            new RetributionStatUp(buffDuration, statModifier)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new AdditionalActionEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }

            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a friendly unit in range!", 50);
            }
        }
    }
}