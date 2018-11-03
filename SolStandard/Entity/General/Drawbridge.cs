using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Drawbridge : TerrainEntity, IOpenable, ILockable
    {
        public bool IsOpen { get; private set; }
        public bool IsLocked { get; private set; }
        private static readonly Color InactiveColor = new Color(100, 100, 100, 50);

        public Drawbridge(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isOpen) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            ElementColor = InactiveColor;
            IsOpen = isOpen;
            IsLocked = true;
            CanMove = false;
        }

        public void Open()
        {
            ElementColor = Color.White;
            Visible = true;
            IsOpen = true;
            CanMove = true;
            AssetManager.DoorSFX.Play();
        }

        public void Close()
        {
            ElementColor = InactiveColor;
            Visible = false;
            IsOpen = false;
            CanMove = false;
            AssetManager.DoorSFX.Play();
        }

        public void ToggleLock()
        {
            AssetManager.UnlockSFX.Play();
            IsLocked = !IsLocked;
        }


        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            InfoHeader,
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (IsLocked) ? "Locked" : "Unlocked",
                                (IsLocked) ? NegativeColor : PositiveColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (IsOpen) ? "Open" : "Closed",
                                (IsOpen) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}