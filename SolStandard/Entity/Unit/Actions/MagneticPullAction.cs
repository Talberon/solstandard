using System.Collections.Generic;
using SolStandard.Containers;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General.Item;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Map.Elements;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;

namespace SolStandard.Entity.Unit.Actions
{
    public class MagneticPullAction : UnitAction
    {
        private readonly Magnet magnet;

        public MagneticPullAction(Magnet magnet, IRenderable skillIcon, int[] skillRange) :
            base(
                icon: skillIcon,
                name: "Pull Unit",
                description: "Pull target unit towards you.",
                tileSprite: MapDistanceTile.GetTileSprite(MapDistanceTile.TileType.Action),
                range: skillRange,
                freeAction: false
            )
        {
            this.magnet = magnet;
        }

        public override void ExecuteAction(MapSlice targetSlice)
        {
            GameUnit targetUnit = UnitSelector.SelectUnit(targetSlice.UnitEntity);

            if (!magnet.IsBroken)
            {
                if (TargetIsUnitInRange(targetSlice, targetUnit))
                {
                    if (Challenge.CanPull(targetSlice, targetUnit))
                    {
                        magnet.DecrementRemainingUses();

                        MapContainer.ClearDynamicAndPreviewGrids();

                        Queue<IEvent> eventQueue = new Queue<IEvent>();
                        
                        //FIXME If a unit is diagonal from the magnet user, the unit should not be placed inside the user's space
                        eventQueue.Enqueue(new PullEvent(targetUnit));
                        
                        eventQueue.Enqueue(new WaitFramesEvent(10));
                        eventQueue.Enqueue(new AdditionalActionEvent());
                        GlobalEventQueue.QueueEvents(eventQueue);
                    }
                    else
                    {
                        GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Obstructed/Immovable!", 50);
                        AssetManager.WarningSFX.Play();
                    }
                }
                else
                {
                    GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Not a unit in range!", 50);
                    AssetManager.WarningSFX.Play();
                }
            }
            else
            {
                GameContext.GameMapContext.MapContainer.AddNewToastAtMapCursor("Item is broken!", 50);
                AssetManager.WarningSFX.Play();
            }
        }
    }
}