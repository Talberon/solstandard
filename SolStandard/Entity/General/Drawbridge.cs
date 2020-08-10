using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Drawbridge : TerrainEntity, IOpenable, ILockable, IRemotelyTriggerable
    {
        public bool IsOpen { get; private set; }
        public bool IsLocked { get; private set; }
        private static readonly Color InactiveColor = new Color(180, 180, 180, 100);

        public Drawbridge(string name, string type, IRenderable sprite, Vector2 mapCoordinates, bool isOpen) :
            base(name, type, sprite, mapCoordinates)
        {
            ElementColor = InactiveColor;
            IsOpen = isOpen;
            IsLocked = true;
            CanMove = false;

            if (IsOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        public void Open()
        {
            ElementColor = Color.White;
            Visible = true;
            IsOpen = true;
            CanMove = true;
        }

        public void Close()
        {
            ElementColor = InactiveColor;
            Visible = false;
            IsOpen = false;
            CanMove = false;
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

        public void ToggleLock()
        {
            AssetManager.UnlockSFX.Play();
            IsLocked = !IsLocked;
        }


        protected override IRenderable EntityInfo => new RenderText(
            AssetManager.WindowFont,
            (IsOpen) ? "Open" : "Closed",
            (IsOpen) ? PositiveColor : NegativeColor
        );

        public void RemoteTrigger()
        {
            GlobalContext.MapCursor.SnapCameraAndCursorToCoordinates(MapCoordinates);
            GlobalContext.MapCamera.SnapCameraCenterToCursor();
            GlobalContext.WorldContext.MapContainer.AddNewToastAtMapCursor(Name + " triggered!", 50);

            ToggleOpen();
        }

        public bool IsObstructed
        {
            get
            {
                MapSlice bridgeSlice = MapContainer.GetMapSliceAtCoordinates(MapCoordinates);
                return bridgeSlice.UnitEntity != null;
            }
        }
    }
}