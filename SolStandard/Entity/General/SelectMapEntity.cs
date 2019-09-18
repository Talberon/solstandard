﻿using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Entity.General
{
    public class SelectMapEntity : TerrainEntity
    {
        public readonly MapInfo MapInfo;
        public readonly MapObjectives MapObjectives;
        public readonly string MapSongName;
        public readonly bool Draft;
        public readonly int MaxBlueUnits;
        public readonly int MaxRedUnits;
        public readonly int MaxDuplicateUnits;
        private readonly IRenderable mapPreview;
        public readonly Team SoloTeam;

        private static readonly Vector2 MaximumPreviewSize = new Vector2(300, 200);

        public SelectMapEntity(string name, string type, IRenderable sprite, Vector2 mapCoordinates, MapInfo mapInfo,
            string mapSongName, MapObjectives mapObjectives, bool draft, int maxBlueUnits, int maxRedUnits,
            int maxDuplicateUnits, Team soloTeam, ITexture2D mapPreview) :
            base(name, type, sprite, mapCoordinates)
        {
            MapInfo = mapInfo;
            MapSongName = mapSongName;
            MapObjectives = mapObjectives;
            Draft = draft;
            MaxBlueUnits = maxBlueUnits;
            MaxRedUnits = maxRedUnits;
            MaxDuplicateUnits = maxDuplicateUnits;
            SoloTeam = soloTeam;
            this.mapPreview = (mapPreview == null)
                ? RenderBlank.Blank
                : new SpriteAtlas(mapPreview, new Vector2(mapPreview.Width, mapPreview.Height),
                    FitImageToSize(MaximumPreviewSize, mapPreview));
        }

        public override IRenderable TerrainInfo =>
            new WindowContentGrid(
                new[,]
                {
                    {new RenderText(AssetManager.HeaderFont, MapInfo.Title)},
                    {TeamUnitCountContent},
                    {MapObjectives.Preview},
                    {mapPreview}
                },
                3,
                HorizontalAlignment.Centered
            );

        private IRenderable TeamUnitCountContent => new WindowContentGrid(
            new IRenderable[,]
            {
                {
                    new Window(
                        new IRenderable[,]
                        {
                            {
                                TeamIconProvider.GetTeamIcon(Team.Blue, GameDriver.CellSizeVector),
                                new RenderText(AssetManager.WindowFont, $" Blue: {MaxBlueUnits}")
                            }
                        },
                        TeamUtility.DetermineTeamColor(Team.Blue)
                    ),
                    new Window(
                        new IRenderable[,]
                        {
                            {
                                TeamIconProvider.GetTeamIcon(Team.Red, GameDriver.CellSizeVector),
                                new RenderText(AssetManager.WindowFont, $" Red: {MaxRedUnits}")
                            }
                        },
                        TeamUtility.DetermineTeamColor(Team.Red)
                    )
                }
            },
            2,
            HorizontalAlignment.Centered
        );

        private static Vector2 FitImageToSize(Vector2 maximumSize, ITexture2D sourceImage)
        {
            Vector2 imageSize = new Vector2(sourceImage.Width, sourceImage.Height);

            if (!(imageSize.X > maximumSize.X || imageSize.Y > maximumSize.Y)) return imageSize;

            bool widerThanTall = imageSize.X > imageSize.Y;
            if (widerThanTall)
            {
                float newHeight = imageSize.Y * maximumSize.X / imageSize.X;
                return new Vector2(maximumSize.X, newHeight);
            }

            float newWidth = imageSize.X * maximumSize.Y / imageSize.Y;
            return new Vector2(newWidth, maximumSize.Y);
        }
    }
}