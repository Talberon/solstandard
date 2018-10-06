using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Skills;
using SolStandard.Entity.Unit.Skills.Terrain;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class Door : TerrainEntity, IActionTile, IOpenable, ILockable
    {
        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }
        public int[] Range { get; private set; }

        public Door(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isLocked, bool isOpen, int[] range, bool canMove) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            IsLocked = isLocked;
            IsOpen = isOpen;
            Range = range;
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
                            Sprite,
                            new RenderText(AssetManager.HeaderFont, Name)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "~~~~~~~~~~~"),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (CanMove) ? "Can Move" : "No Move",
                                (CanMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (IsLocked) ? "Locked" : "Unlocked",
                                (IsLocked) ? PositiveColor : NegativeColor),
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

        public UnitSkill TileAction()
        {
            return new UseDoorSkill(MapCoordinates);
        }

        public void Open()
        {
            if (!IsLocked)
            {
                //TODO Have a frame for the door in its open state
                //TODO Play open door SFX
                Visible = false;
                IsOpen = true;
                CanMove = true;
            }
            else
            {
                //TODO Play locked SFX
            }
        }

        public void Close()
        {
            //TODO Play close door sfx
            Visible = true;
            IsOpen = false;
            CanMove = false;
        }

        public void Unlock()
        {
            //TODO Play unlock SFX
            IsLocked = false;
        }
    }
}