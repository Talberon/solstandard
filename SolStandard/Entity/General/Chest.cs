using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Chest : TerrainEntity
    {
        private readonly string contents;
        private bool isLocked;
        private bool isOpen;

        public Chest(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, string contents, bool isLocked, bool isOpen) : base(name, type,
            sprite, mapCoordinates, tiledProperties)
        {
            this.contents = contents;
            this.isLocked = isLocked;
            this.isOpen = isOpen;
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
                            new RenderText(GameDriver.HeaderFont, Name)
                        },
                        {
                            new RenderText(GameDriver.WindowFont, "~~~~~~~~~~~"),
                            new RenderBlank()
                        },
                        {
                            new RenderText(GameDriver.WindowFont, (isLocked) ? "Locked" : "Unlocked",
                                (isLocked) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(GameDriver.WindowFont, (isOpen) ? "Open" : "Closed",
                                (isOpen) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        },
                        {
                            new RenderText(GameDriver.WindowFont, "Contents: "),
                            new RenderText(GameDriver.WindowFont, (isOpen) ? contents : "????")
                        }
                    },
                    3
                );
            }
        }
    }
}