﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class TerrainEntity : MapEntity
    {
        protected static readonly Color PositiveColor = new Color(30, 200, 30);
        protected static readonly Color NegativeColor = new Color(250, 10, 10);
        
        public TerrainEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties) : base(name, type, sprite, mapCoordinates, tiledProperties)
        {
        }

        public virtual IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new [,]
                    {
                        {
                            Sprite
                        },
                        {
                            new RenderText(GameDriver.WindowFont, Name)
                        }
                    },
                    1
                );
            }
        }
    }
}