using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class ToggleSwitchAction : UnitAction
    {
        private readonly List<IRemotelyTriggerable> targetTriggerables;
        private readonly Switch switchTile;

        public ToggleSwitchAction(Switch switchTile, List<IRemotelyTriggerable> targetTriggerables) : base(
            icon: switchTile.RenderSprite,
            name: "Use: " + switchTile.Name,
            description: "Opens or closes the target triggerable.",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.switchTile = switchTile;
            this.targetTriggerables = targetTriggerables;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) switchTile.MapCoordinates.X, (int) switchTile.MapCoordinates.Y] =
                new MapDistanceTile(TileSprite, switchTile.MapCoordinates);
            
            GameContext.GameMapContext.MapContainer.MapCursor.SnapCursorToCoordinates(switchTile.MapCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetingSwitch(targetSlice) && NothingObstructingSwitchTarget())
            {
                switchTile.ToggleActive();

                MapContainer.ClearDynamicAndPreviewGrids();
                Queue<IEvent> eventQueue = new Queue<IEvent>();
                foreach (IRemotelyTriggerable triggerable in targetTriggerables)
                {
                    eventQueue.Enqueue(new TriggerEntityEvent(triggerable));
                }

                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                if (!TargetingSwitch(targetSlice))
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a target switch!", 50);
                }

                if (!NothingObstructingSwitchTarget())
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Switch target is obstructed!", 50);
                }

                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetingSwitch(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null &&
                   targetSlice.TerrainEntity is Switch;
        }

        private bool NothingObstructingSwitchTarget()
        {
            //Don't allow the switch to be triggered if any target tiles have a unit on them
            foreach (IRemotelyTriggerable triggerable in targetTriggerables)
            {
                TerrainEntity entity = triggerable as TerrainEntity;
                if (entity == null) continue;

                if (MapContainer.GetMapSliceAtCoordinates(entity.MapCoordinates).UnitEntity != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}