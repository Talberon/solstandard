using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class Movable : TerrainEntity
    {
        private readonly bool canMove;

        public Movable(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, bool canMove) : base(name, type, sprite, mapCoordinates,
            tiledProperties)
        {
            this.canMove = canMove;
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
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(GameDriver.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? PositiveColor : NegativeColor)
                        }
                    },
                    3
                );
            }
        }
    }
}