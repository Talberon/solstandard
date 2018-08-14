using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Objects;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Rules
{
    public class MapStaticHud
    {
        private readonly ISpriteFont windowFont;
        private readonly ITexture2D windowTexture;

        public MapStaticHud(ISpriteFont windowFont, ITexture2D windowTexture)
        {
            this.windowFont = windowFont;
            this.windowTexture = windowTexture;
        }


        public Window GenerateTurnWindow(Vector2 windowSize)
        {
            WindowContentGrid unitListContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        new RenderText(windowFont,
                            "EXAMPLE//Current Turn: 0") //TODO make dynamic; not hard-coded
                    },
                    {
                        new RenderText(windowFont,
                            "EXAMPLE//Active Team: Blue") //TODO make dynamic; not hard-coded
                    },
                    {
                        new RenderText(windowFont,
                            "EXAMPLE//Active Unit: Knight") //TODO make dynamic; not hard-coded
                    }
                },
                1);

            return new Window("Turn Counter", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225),
                windowSize);
        }

        public Window GenerateTerrainWindow(Vector2 cursorCoordinates, MapEntity selectedTerrain)
        {
            WindowContentGrid terrainContentGrid;

            if (selectedTerrain != null)
            {
                IRenderable terrainSprite = selectedTerrain.GetSprite();

                string terrainInfo = "Terrain: " + selectedTerrain.Name
                                                 + "\n"
                                                 + "Type: " + selectedTerrain.Type
                                                 + "\n"
                                                 + "Properties:\n" + string.Join("\n",
                                                     selectedTerrain.TiledProperties);

                terrainContentGrid = new WindowContentGrid(
                    new[,]
                    {
                        {
                            terrainSprite,
                            new RenderText(windowFont, terrainInfo)
                        }
                    },
                    1);
            }
            else
            {
                terrainContentGrid = new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(windowFont, "None ")
                        }
                    },
                    1);
            }

            return new Window("Terrain Info", windowTexture, terrainContentGrid, new Color(100, 150, 100, 220));
        }

        public Window GenerateHelpWindow(string helpText)
        {
            IRenderable textToRender = new RenderText(windowFont, helpText);
            return new Window("Help Text", windowTexture, textToRender, new Color(30, 30, 30, 150));
        }

        public Window GenerateInitiativeWindow(List<GameUnit> unitList)
        {
            const int
                maxInitiativeSize =
                    10; //TODO figure out if we really want this to be hard-coded or determined based on screen size or something

            int initiativeListLength = (unitList.Count > maxInitiativeSize) ? maxInitiativeSize : unitList.Count;

            IRenderable[,] unitListGrid = new IRenderable[1, initiativeListLength];

            for (int i = 0; i < unitListGrid.GetLength(1); i++)
            {
                IRenderable unitPortraitWindow = new WindowContent(
                    new TileCell(
                        unitList[i].MediumPortrait,
                        unitList[i].MediumPortrait.GetHeight(),
                        1
                    )
                );
                unitListGrid[0, i] = unitPortraitWindow;
            }

            WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListGrid, 3);

            return new Window("Initiative", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225));
        }

        public Window GenerateUnitPortraitWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            IRenderable selectedUnitPortrait =
                new WindowContent(new TileCell(selectedUnit.LargePortrait, selectedUnit.LargePortrait.GetHeight(), 1));

            string windowLabel = "Selected Portrait: " + selectedUnit.Id;

            Color windowColour = DetermineTeamColor(selectedUnit.UnitTeam);

            return new Window(windowLabel, windowTexture, selectedUnitPortrait, windowColour);
        }

        public Window GenerateUnitDetailWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            IRenderable selectedUnitInfo = new RenderText(windowFont, selectedUnit.Id + ":\n" + selectedUnit.Stats);

            string windowLabel = "Selected Info: " + selectedUnit.Id;

            Color windowColour = DetermineTeamColor(selectedUnit.UnitTeam);

            return new Window(windowLabel, windowTexture, selectedUnitInfo, windowColour);
        }

        private static Color DetermineTeamColor(Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    return new Color(75, 75, 150, 200);
                case Team.Red:
                    return new Color(150, 75, 75, 200);
                default:
                    return new Color(75, 150, 75, 200);
            }
        }
    }
}