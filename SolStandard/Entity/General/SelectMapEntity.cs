using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Entity.General
{
    public class SelectMapEntity : TerrainEntity
    {
        public readonly MapInfo MapInfo;
        public readonly MapObjectives MapObjectives;
        public readonly string MapSongName;
        public readonly TurnOrder TurnOrder;

        public SelectMapEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, MapInfo mapInfo, string mapSongName,
            MapObjectives mapObjectives, string turnOrder) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            MapInfo = mapInfo;
            MapSongName = mapSongName;
            MapObjectives = mapObjectives;

            if (!Enum.TryParse(turnOrder, out TurnOrder))
            {
                throw new InvalidDataException("Turn order " + turnOrder + " does not map to a TurnOrder enum value.");
            }
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {
                            new RenderText(AssetManager.HeaderFont, MapInfo.Title),
                            new RenderBlank()
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "Turn Order: "),
                            new RenderText(AssetManager.WindowFont, TurnOrder.ToString(), new Color(250, 5, 170))
                        },
                        {
                            MapObjectives.Preview,
                            new RenderBlank()
                        }
                    },
                    3
                );
            }
        }
    }
}