using System;
using System.Collections.Generic;
using SolStandard.Containers.Components.Global;
using SolStandard.Map;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class Guard : UnitAction
    {
        private readonly int armorPoints;

        public Guard(int armorPoints) : base(
            icon: UnitStatistics.GetSpriteAtlas(Stats.Armor, GameDriver.CellSizeVector),
            name: $"Guard [{armorPoints}]",
            description: "Regenerate [" + armorPoints + "] " + UnitStatistics.Abbreviation[Stats.Armor] + ".",
            tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
            range: new[] {0},
            freeAction: false
        )
        {
            this.armorPoints = armorPoints;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (TargetIsSelfInRange(targetSlice, targetUnit))
            {
                MapContainer.ClearDynamicAndPreviewGrids();

                var eventQueue = new Queue<IEvent>();
                eventQueue.Enqueue(
                    new PlayAnimationAtCoordinatesEvent(AnimatedIconType.Interact, targetSlice.MapCoordinates)
                );
                eventQueue.Enqueue(new RegenerateArmorEvent(targetUnit, armorPoints));
                eventQueue.Enqueue(new EndTurnEvent());
                GlobalEventQueue.QueueEvents(eventQueue);

                string toastMessage = "Guard!" + Environment.NewLine +
                                      "Recovered [" + armorPoints + "] " + UnitStatistics.Abbreviation[Stats.Armor] +
                                      "!";
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(toastMessage, 50);
            }
            else
            {
                GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor("Invalid target!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}