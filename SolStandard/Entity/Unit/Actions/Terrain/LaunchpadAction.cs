using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Targeting;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit.Actions.Mage;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Terrain
{
    public class LaunchpadAction : UnitAction
    {
        private readonly Launchpad launchpad;

        public LaunchpadAction(Launchpad launchpad, int[] range) :
            base(
                icon: SkillIconProvider.GetSkillIcon(SkillIcon.Jump, GameDriver.CellSizeVector),
                name: "Launch",
                description: "Jump to a tile in range that can be landed on.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: range,
                freeAction: false
            )
        {
            this.launchpad = launchpad;
        }

        public override void GenerateActionGrid(Vector2 origin, Layer mapLayer = Layer.Dynamic)
        {
            var unitTargetingContext = new UnitTargetingPhase(TileSprite);
            unitTargetingContext.GenerateTargetingGrid(origin, Range, mapLayer);
            Blink.RemoveActionTilesOnUnmovableSpaces(mapLayer);
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (CanMoveToTargetTile(targetSlice))
            {
                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new PlayEntityAnimationOnceEvent(launchpad));
                eventQueue.Enqueue(new MoveEntityToCoordinatesEvent(GlobalContext.ActiveUnit.UnitEntity,
                    targetSlice.MapCoordinates));
                eventQueue.Enqueue(new PlaySoundEffectEvent(AssetManager.DoorSFX));
                eventQueue.Enqueue(new WaitFramesEvent(10));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Can't land here!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}