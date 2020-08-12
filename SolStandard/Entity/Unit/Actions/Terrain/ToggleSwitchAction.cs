using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
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
            range: new[] {1},
            freeAction: true
        )
        {
            this.switchTile = switchTile;
            this.targetTriggerables = targetTriggerables;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            MapContainer.GameGrid[(int) mapLayer][(int) switchTile.MapCoordinates.X,
                    (int) switchTile.MapCoordinates.Y] =
                new MapDistanceTile(TileSprite, switchTile.MapCoordinates);

            GlobalContext.WorldContext.MapContainer.MapCursor.SnapCameraAndCursorToCoordinates(
                switchTile.MapCoordinates);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetingSwitch(targetSlice) && NothingObstructingSwitchTarget(targetTriggerables))
            {
                switchTile.ToggleActive();
                AssetManager.DoorSFX.Play();

                MapContainer.ClearDynamicAndPreviewGrids();
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                foreach (IRemotelyTriggerable triggerable in targetTriggerables)
                {
                    eventQueue.Enqueue(new TriggerEntityEvent(triggerable));
                }

                eventQueue.Enqueue(new WaitFramesEvent(50));

                if (IsCreepTurn)
                {
                    eventQueue.Enqueue(new EndTurnEvent());
                }
                else
                {
                    eventQueue.Enqueue(new AdditionalActionEvent());
                }

                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                if (!TargetingSwitch(targetSlice))
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not a target switch!", 50);
                }

                if (!NothingObstructingSwitchTarget(targetTriggerables))
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Switch target is obstructed!", 50);
                }

                AssetManager.WarningSFX.Play();
            }
        }

        private static bool IsCreepTurn => GlobalContext.ActiveTeam == Team.Creep;

        private static bool TargetingSwitch(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null &&
                   targetSlice.TerrainEntity is Switch;
        }


        public static bool NothingObstructingSwitchTarget(IEnumerable<IRemotelyTriggerable> targetTriggerables)
        {
            return targetTriggerables.OfType<IOpenable>().All(openable => !openable.IsObstructed);
        }
    }
}