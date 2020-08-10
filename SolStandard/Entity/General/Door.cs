using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Events;
using SolStandard.Utility.Events.AI;

namespace SolStandard.Entity.General
{
    public class Door : BreakableObstacle, IActionTile, IOpenable, ILockable, ITriggerable, IRemotelyTriggerable
    {
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] InteractRange { get; }
        private static readonly Color InactiveColor = new Color(0, 0, 0, 50);

        public Door(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool isLocked, bool isOpen,
            int[] range, int hp) :
            base(name, type, sprite, mapCoordinates, hp, isOpen, false, 0)
        {
            IsLocked = isLocked;
            InteractRange = range;
            IsOpen = isOpen;
            ElementColor = (IsOpen) ? InactiveColor : Color.White;
        }

        protected override IRenderable EntityInfo =>
            new WindowContentGrid(new[,]
                {
                    {
                        new SpriteAtlas(
                            AssetManager.LockTexture,
                            new Vector2(AssetManager.LockTexture.Width),
                            GameDriver.CellSizeVector,
                            Convert.ToInt32(
                                IsLocked ? Chest.LockIconState.Locked : Chest.LockIconState.Unlocked
                            )
                        ),
                        new RenderText(AssetManager.WindowFont, (IsLocked) ? "Locked" : "Unlocked",
                            (IsLocked) ? NegativeColor : PositiveColor)
                    },
                    {
                        new RenderText(AssetManager.WindowFont, (IsOpen) ? "Open" : "Closed",
                            (IsOpen) ? PositiveColor : NegativeColor),
                        RenderBlank.Blank
                    },
                    {
                        base.EntityInfo,
                        RenderBlank.Blank
                    },
                },
                1,
                HorizontalAlignment.Centered
            );

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new UseDoorAction(this, MapCoordinates)
            };
        }


        public void Trigger()
        {
            if (!CanTrigger) return;
            UnitAction toggleAction = new UseDoorAction(this, MapCoordinates, false);
            toggleAction.GenerateActionGrid(GlobalContext.ActiveUnit.UnitEntity.MapCoordinates);
            toggleAction.ExecuteAction(MapContainer.GetMapSliceAtCoordinates(MapCoordinates));
            GlobalEventQueue.QueueSingleEvent(new CreepEndTurnEvent());
            MapContainer.ClearDynamicAndPreviewGrids();
        }

        public void RemoteTrigger()
        {
            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GlobalContext.MapCamera.SnapCameraCenterToCursor();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(Name + " triggered!", 50);
            ToggleOpen();
        }

        private void ToggleOpen()
        {
            if (IsOpen)
            {
                Close();
            }

            else
            {
                Open();
            }
        }

        public void Open()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = InactiveColor;
            IsOpen = true;
            CanMove = true;
        }

        public void Close()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = Color.White;
            IsOpen = false;
            CanMove = false;
        }

        public void ToggleLock()
        {
            AssetManager.UnlockSFX.Play();
            IsLocked = !IsLocked;
        }

        public bool IsObstructed
        {
            get
            {
                MapSlice doorSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
                return doorSlice.UnitEntity != null;
            }
        }

        public bool CanTrigger => !IsLocked;
    }
}