using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit.Skills;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    //TODO Implement IActionTile
    public class Door : TerrainEntity 
    {
        private bool isLocked;
        private bool isOpen;
        public int[] Range { get; private set; }

        public Door(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool isLocked, bool isOpen, int[] range) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.isLocked = isLocked;
            this.isOpen = isOpen;
            Range = range;
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
                            new RenderText(AssetManager.WindowFont, (isLocked) ? "Locked" : "Unlocked",
                                (isLocked) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (isOpen) ? "Open" : "Closed",
                                (isOpen) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }

        public UnitSkill TileAction()
        {
            throw new NotImplementedException();
        }
    }
}