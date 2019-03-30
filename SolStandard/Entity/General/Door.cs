using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Terrain;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Door : BreakableObstacle, IActionTile, IOpenable, ILockable, IRemotelyTriggerable
    {
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] InteractRange { get; private set; }
        private static readonly Color InactiveColor = new Color(0, 0, 0, 50);

        public Door(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isLocked, bool isOpen, int[] range, int hp, bool canMove) :
            base(name, type, sprite, mapCoordinates, tiledProperties, hp, canMove, false, 0)
        {
            IsLocked = isLocked;
            IsOpen = isOpen;
            InteractRange = range;
            CanMove = canMove;
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
                            new Window(new IRenderable[,]
                                {
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
                                        new Window(new IRenderable[,]
                                            {
                                                {
                                                    UnitStatistics.GetSpriteAtlas(Stats.Hp),
                                                    new RenderText(AssetManager.WindowFont, "HP: " + HP)
                                                },
                                                {
                                                    new RenderText(AssetManager.WindowFont,
                                                        (IsBroken) ? "Broken" : "Not Broken",
                                                        (IsBroken) ? NegativeColor : PositiveColor),
                                                    new RenderBlank()
                                                }
                                            }, 
                                            InnerWindowColor,
                                            HorizontalAlignment.Centered
                                        ),
                                        new RenderBlank()
                                    }
                                },
                                InnerWindowColor,
                                HorizontalAlignment.Centered
                            ),
                            new RenderBlank()
                        }
                    },
                    3,
                    HorizontalAlignment.Centered
                );
            }
        }

        public List<UnitAction> TileActions()
        {
            return new List<UnitAction>
            {
                new UseDoorAction(this, MapCoordinates)
            };
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

        public void RemoteTrigger()
        {
            ToggleOpen();
        }
    }
}