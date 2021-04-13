using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.Network;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.Unit.Actions
{
    public class SpawnUnitAction : UnitAction
    {
        private readonly Role unitRole;
        private readonly IItem spawnItem;

        public SpawnUnitAction(Role unitRole, IItem spawnItem = null) : base(
            icon: UnitIcon(unitRole),
            name: "Spawn " + unitRole,
            description: "Spawn a new ally with the [" + unitRole + "] role!",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1},
            freeAction: false
        )
        {
            this.unitRole = unitRole;
            this.spawnItem = spawnItem;
        }

        private static IRenderable UnitIcon(Role role)
        {
            ITexture2D unitPortrait = UnitGenerator.GetUnitPortrait(role,
                (GlobalContext.ActiveUnit != null) ? GlobalContext.ActiveTeam : Team.Blue);
            return new SpriteAtlas(unitPortrait,
                new Vector2(unitPortrait.Width, unitPortrait.Height),
                GameDriver.CellSizeVector
            );
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsUnoccupiedTileInRange(targetSlice))
            {
                if (spawnItem != null) GlobalContext.ActiveUnit.RemoveItemFromInventory(spawnItem);

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(
                    new SpawnUnitEvent(
                        unitRole,
                        GlobalContext.ActiveTeam,
                        targetSlice.MapCoordinates
                    )
                );
                eventQueue.Enqueue(new WaitFramesEvent(50));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        public static void PlaceUnitInTile(Role role, Team team, Vector2 mapCoordinates)
        {
            GameUnit unitToSpawn = UnitGenerator.GenerateAdHocUnit(role, team, false);
            unitToSpawn.UnitEntity.SnapToCoordinates(mapCoordinates);
            unitToSpawn.ExhaustAndDisableUnit();
            GlobalContext.Units.Add(unitToSpawn);
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Spawned new " + role + "!", 50);
            AssetManager.SkillBuffSFX.Play();
        }

        public static bool TargetIsUnoccupiedTileInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null && targetSlice.UnitEntity == null &&
                   UnitMovingPhase.CanEndMoveAtCoordinates(targetSlice.MapCoordinates);
        }
    }
}