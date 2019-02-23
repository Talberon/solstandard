using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions.Mage
{
    public class Inferno : LayTrap
    {
        public Inferno(int damage, int maxTriggers) : base(
            skillIcon: SkillIconProvider.GetSkillIcon(SkillIcon.Inferno, new Vector2(GameDriver.CellSize)),
            trapSprite: new AnimatedSpriteSheet(
                AssetManager.FireTexture, AssetManager.FireTexture.Height, new Vector2(GameDriver.CellSize), 6, false,
                Color.White
            ),
            title: "Inferno",
            damage: damage,
            maxTriggers: maxTriggers,
            description: "Place up to 4 tiles around you that will deal [" + damage +
                         "] damage to enemies that start their turn on it." + Environment.NewLine +
                         "Max activations: [" + maxTriggers + "]"
        )
        {
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            if (TargetIsInRange(targetSlice))
            {
                bool allTilesObstructed = true;

                Queue<IEvent> eventQueue = new Queue<IEvent>();

                foreach (MapElement targetTile in MapContainer.GetMapElementsFromLayer(Layer.Dynamic))
                {
                    MapSlice slice = MapContainer.GetMapSliceAtCoordinates(targetTile.MapCoordinates);

                    if (!TargetIsNotObstructed(slice)) continue;

                    TrapEntity trap = new TrapEntity("Fire", TrapSprite.Clone(), slice.MapCoordinates, Damage,
                        MaxTriggers, true, true);

                    MapContainer.ClearDynamicAndPreviewGrids();

                    eventQueue.Enqueue(new PlaceEntityOnMapEvent(trap, Layer.Entities, AssetManager.DropItemSFX));
                    allTilesObstructed = false;
                }

                if (allTilesObstructed)
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("All tiles are obstructed!", 50);
                    AssetManager.WarningSFX.Play();
                }
                else
                {
                    eventQueue.Enqueue(new EndTurnEvent());
                    GlobalEventQueue.QueueEvents(eventQueue);
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not in range!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}