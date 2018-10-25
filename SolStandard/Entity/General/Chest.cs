using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Chest : TerrainEntity, IActionTile, IOpenable, ILockable
    {
        public int Gold { get; private set; }
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] Range { get; private set; }
        private static readonly Color InactiveColor = new Color(50, 50, 50);

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isLocked, bool isOpen, bool canMove, int[] range,
            int gold) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            CanMove = canMove;
            IsLocked = isLocked;
            IsOpen = isOpen;
            Range = range;
            Gold = gold;
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
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
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Gold: "),
                            new RenderText(AssetManager.WindowFont,
                                (IsOpen) ? Gold + Currency.CurrencyAbbreviation : "????")
                        }
                    },
                    3
                );
            }
        }

        public UnitAction TileAction()
        {
            return new OpenChestAction(this, MapCoordinates);
        }

        public void Open()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = InactiveColor;
            IsOpen = true;
        }

        public void Close()
        {
            AssetManager.DoorSFX.Play();
            ElementColor = Color.White;
            IsOpen = false;
        }

        public void ToggleLock()
        {
            AssetManager.UnlockSFX.Play();
            IsLocked = !IsLocked;
        }
    }
}