using System;
using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class LayTrap : UnitAction
    {
        protected readonly IRenderable TrapSprite;
        protected readonly int Damage;
        protected readonly int MaxTriggers;
        private readonly TrapEntity trapItem;

        protected LayTrap(IRenderable skillIcon, IRenderable trapSprite, string title, int damage, int maxTriggers,
            int[] range, string description = null, bool freeAction = false)
            : base(
                icon: skillIcon.Clone(),
                name: title,
                description: description ??
                             ($"Place a tile that will deal [{damage}] damage to units that start their turn on it." +
                              Environment.NewLine +
                              $"Max activations: [{maxTriggers}]"),
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: range,
                freeAction: freeAction
            )
        {
            Damage = damage;
            MaxTriggers = maxTriggers;
            TrapSprite = trapSprite;
        }

        public LayTrap(TrapEntity trapItem)
            : base(
                icon: trapItem.RenderSprite.Clone(),
                name: "Place Trap",
                description:
                $"Place a tile that will deal [{trapItem.Damage}] damage to units that start their turn on it." +
                Environment.NewLine +
                $"Max activations: [{trapItem.TriggersRemaining}]",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: new[] {1},
                freeAction: true
            )
        {
            this.trapItem = trapItem;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsInRange(targetSlice))
            {
                if (!TargetHasEntityOrWall(targetSlice))
                {
                    TrapEntity trapToPlace;

                    if (trapItem != null)
                    {
                        GlobalContext.ActiveUnit.RemoveItemFromInventory(trapItem);
                        trapItem.SnapToCoordinates(targetSlice.MapCoordinates);
                        trapToPlace = trapItem;
                    }
                    else
                    {
                        trapToPlace = new TrapEntity("Trap", TrapSprite.Clone(), targetSlice.MapCoordinates, Damage,
                            MaxTriggers, true, true, true);
                    }

                    MapContainer.ClearDynamicAndPreviewGrids();
                    var eventQueue = new Queue<IEvent>();
                    eventQueue.Enqueue(
                        new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                    );
                    eventQueue.Enqueue(new PlaceEntityOnMapEvent((TrapEntity) trapToPlace.Duplicate(), Layer.Entities,
                        AssetManager.DropItemSFX));

                    if (FreeAction)
                    {
                        eventQueue.Enqueue(new AdditionalActionEvent());
                    }
                    else
                    {
                        eventQueue.Enqueue(new EndTurnEvent());
                    }

                    GlobalEventQueue.QueueEvents(eventQueue);
                }
                else
                {
                    GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Target is obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Not in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }

        protected static bool TargetHasEntityOrWall(MapSlice targetSlice)
        {
            return (targetSlice.TerrainEntity != null) || (targetSlice.CollideTile != null);
        }

        protected static bool TargetIsInRange(MapSlice targetSlice)
        {
            return targetSlice.DynamicEntity != null;
        }
    }
}