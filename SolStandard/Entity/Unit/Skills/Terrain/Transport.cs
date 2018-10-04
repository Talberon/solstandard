using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Skills.Terrain
{
    public class Transport : UnitSkill
    {
        private readonly Vector2 targetCoordinates;

        public Transport(Vector2 targetCoordinates) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Blink, new Vector2(32)),
            name: "Transport",
            description: "Moves unit to another space.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Movement),
            range: null
        )
        {
            this.targetCoordinates = targetCoordinates;
        }

        public override void GenerateActionGrid(Vector2 origin)
        {
            MapContainer.GameGrid[(int) Layer.Dynamic][(int) targetCoordinates.X, (int) targetCoordinates.Y] =
                new MapDistanceTile(TileSprite, targetCoordinates, 0, false);
        }

        public override void ExecuteAction(MapSlice targetSlice, MapContext mapContext, BattleContext battleContext)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                UnitEntity targetEntity = GameContext.ActiveUnit.UnitEntity;

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new HideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new BlinkCoordinatesEvent(
                    GameContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates
                ));
                eventQueue.Enqueue(new UnhideUnitEvent(ref targetEntity));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent(ref mapContext));
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                AssetManager.WarningSFX.Play();
            }
        }
    }
}