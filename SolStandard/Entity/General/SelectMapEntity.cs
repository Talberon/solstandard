using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;

namespace SolStandard.Entity.General
{
    public class SelectMapEntity : TerrainEntity
    {
        public readonly MapInfo MapInfo;

        public SelectMapEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, MapInfo mapInfo) : base(name, type, sprite, mapCoordinates,
            tiledProperties)
        {
            MapInfo = mapInfo;
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(GameDriver.HeaderFont, MapInfo.Title)
                        },
                        {
                            MapInfo.PreviewImage
                        }
                    },
                    3
                );
            }
        }
    }
}