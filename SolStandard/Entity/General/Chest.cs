using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

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
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Contents: "),
                            new RenderText(AssetManager.WindowFont, (isOpen) ? contents : "????")
                        }
                    },
                    3
                );
            }
        }
    }
}