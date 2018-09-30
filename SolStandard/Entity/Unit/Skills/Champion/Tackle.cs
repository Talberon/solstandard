using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Champion
{
    public class Tackle : UnitSkill
    {
        public Tackle() : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Tackle, new Vector2(32)),
            name: "Tackle",
            description: "Shove an enemy if there is an empty space behind them,"
                         + "\nthen follow up by moving into their space and attacking.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Attack),
            range: new[] {1}
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsAnEnemyInRange(targetSlice, targetUnit))
            {
                Vector2 targetOriginalPosition = targetUnit.UnitEntity.MapCoordinates;

                if (Shove.CanShove(targetUnit))
                {
                    MapContainer.ClearDynamicAndPreviewGrids();

                    Queue<IEvent> eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(new ShoveEvent(ref targetUnit));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(new MoveCoordinatesEvent(targetOriginalPosition));
                    eventQueue.Enqueue(new WaitFramesEvent(10));
                    eventQueue.Enqueue(
                        new StartCombatEvent(
                            GameContext.ActiveUnit.UnitEntity.MapCoordinates,
                            GameContext.ActiveUnit.Stats.AtkRange,
                            ref targetUnit,
                            ref mapContext,
                            ref battleContext,
                            TileSprite
                        )
                    );

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }

        private static void MoveToTarget(Vector2 targetCoordinates)
        {
            GameContext.ActiveUnit.UnitEntity.MapCoordinates = targetCoordinates;
        }
    }
}