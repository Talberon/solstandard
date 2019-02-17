using System.Collections.Generic;
using Microsoft.Xna.Framework;
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
        public readonly bool Draft;
        public readonly int UnitsPerTeam;
        public readonly int MaxDuplicateUnits;

        public SelectMapEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates,
            Dictionary<string, string> tiledProperties, MapInfo mapInfo, string mapSongName,
            MapObjectives mapObjectives, bool draft, int unitsPerTeam, int maxDuplicateUnits) :
            base(name, type, sprite, mapCoordinates, tiledProperties)
        {
            MapInfo = mapInfo;
            MapSongName = mapSongName;
            MapObjectives = mapObjectives;
            Draft = draft;
            UnitsPerTeam = unitsPerTeam;
            MaxDuplicateUnits = maxDuplicateUnits;
        }

        public override IRenderable TerrainInfo
        {
            get
            {
                return new WindowContentGrid(
                    new[,]
                    {
                        {new RenderText(AssetManager.HeaderFont, MapInfo.Title)},
                        {MapObjectives.Preview}
                    },
                    3
                );
            }
        }
    }
}