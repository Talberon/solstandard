﻿using System;
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

namespace SolStandard.Entity.Unit.Actions.Archer
{
    public class LayTrap : UnitAction
    {
        private TrapEntity trap;
        private readonly int damage;
        private readonly int maxTriggers;

        public LayTrap(int damage, int maxTriggers) : base(
            icon: SkillIconProvider.GetSkillIcon(SkillIcon.Trap, new Vector2(GameDriver.CellSize)),
            name: "Lay Trap",
            description: "Lay a trap that will deal [" + damage + "] damage to enemies that start their turn on it." +
                         Environment.NewLine + "Max activations: [" + maxTriggers + "]",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {1}
        )
        {
            this.damage = damage;
            this.maxTriggers = maxTriggers;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetHasNoEntitiesPresent(targetSlice))
            {
                trap = new TrapEntity("Trap", Icon, targetSlice.MapCoordinates, damage, maxTriggers);

                MapContainer.ClearDynamicAndPreviewGrids();

                Queue<IEvent> eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(new PlaceEntityOnMapEvent(trap, Layer.Entities, AssetManager.DropItemSFX));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        private static bool TargetHasNoEntitiesPresent(MapSlice targetSlice)
        {
            return targetSlice.TerrainEntity == null;
        }
    }
}