using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class BreakableObstacle : TerrainEntity
    {
        private readonly int hp;
        private bool canMove;
        private bool isBroken;

        public BreakableObstacle(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, int hp, bool canMove, bool isBroken) : base(name, type, sprite,
            mapCoordinates, tiledProperties)
        {
            this.hp = hp;
            this.canMove = canMove;
            this.isBroken = isBroken;
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Hp),
                            new RenderText(AssetManager.WindowFont, "HP: " + hp)
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? PositiveColor : NegativeColor)
                        },
                        {
                            new RenderText(AssetManager.WindowFont, (isBroken) ? "Not Broken" : "Broken",
                                (isBroken) ? PositiveColor : NegativeColor),
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}