using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    /*
     * MapUI is where the HUD elements for the Map Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class MapUI : IUserInterface
    {
        private readonly Vector2 screenSize;
        private const int WindowEdgeBuffer = 5;
        private const int UnitDetailVerticalAdjustment = 3;

        private Window leftUnitPortraitWindow;
        private Window leftUnitDetailWindow;

        private Window rightUnitPortraitWindow;
        private Window rightUnitDetailWindow;

        public Window TurnWindow { get; private set; }
        public Window InitiativeWindow { get; private set; }
        public Window TerrainEntityWindow { get; private set; }
        public Window HelpTextWindow { get; private set; }

        public Window UserPromptWindow { get; private set; }

        private bool visible;

        private readonly ITexture2D windowTexture;

        public MapUI(Vector2 screenSize, ITexture2D windowTexture)
        {
            this.screenSize = screenSize;
            this.windowTexture = windowTexture;
            visible = true;
        }

        public Window LeftUnitPortraitWindow
        {
            get { return leftUnitPortraitWindow; }
        }

        public Window LeftUnitDetailWindow
        {
            get { return leftUnitDetailWindow; }
        }

        public Window RightUnitPortraitWindow
        {
            get { return rightUnitPortraitWindow; }
        }

        public Window RightUnitDetailWindow
        {
            get { return rightUnitDetailWindow; }
        }

        internal void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window("User Prompt Window", windowTexture, promptTextContent, promptWindowColor,
                sizeOverride);
        }

        public void GenerateTurnWindow(Vector2 windowSize)
        {
            WindowContentGrid unitListContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        new RenderText(GameDriver.WindowFont,
                            "EXAMPLE//Current Turn: 0") //TODO make dynamic; not hard-coded
                    },
                    {
                        new RenderText(GameDriver.WindowFont,
                            "EXAMPLE//Active Team: Blue") //TODO make dynamic; not hard-coded
                    },
                    {
                        new RenderText(GameDriver.WindowFont,
                            "EXAMPLE//Active Unit: Knight") //TODO make dynamic; not hard-coded
                    }
                },
                1);

            TurnWindow = new Window("Turn Counter", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225),
                windowSize);
        }

        public void GenerateTerrainWindow(MapEntity selectedTerrain)
        {
            WindowContentGrid terrainContentGrid;

            if (selectedTerrain != null)
            {
                IRenderable terrainSprite = selectedTerrain.RenderSprite;

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
                            new RenderText(GameDriver.WindowFont, terrainInfo)
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
                            new RenderText(GameDriver.WindowFont, "None ")
                        }
                    },
                    1);
            }

            TerrainEntityWindow = new Window("Terrain Info", windowTexture, terrainContentGrid,
                new Color(100, 150, 100, 220));
        }

        public void GenerateHelpWindow(string helpText)
        {
            IRenderable textToRender = new RenderText(GameDriver.WindowFont, helpText);
            HelpTextWindow = new Window("Help Text", windowTexture, textToRender, new Color(30, 30, 30, 150));
        }

        public void GenerateInitiativeWindow(List<GameUnit> unitList)
        {
            const int
                maxInitiativeSize =
                    9; //TODO figure out if we really want this to be hard-coded or determined based on screen size or something

            int initiativeListLength = (unitList.Count > maxInitiativeSize) ? maxInitiativeSize : unitList.Count;

            IRenderable[,] unitListGrid = new IRenderable[1, initiativeListLength];
            const int initiativeHealthBarHeight = 10;

            GenerateFirstUnitInInitiativeList(unitList, initiativeHealthBarHeight, unitListGrid);
            GenerateRestOfInitiativeList(unitList, unitListGrid, initiativeHealthBarHeight);


            WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListGrid, 3);

            InitiativeWindow = new Window("Initiative List", windowTexture, unitListContentGrid,
                new Color(100, 100, 100, 225));
        }

        private void GenerateFirstUnitInInitiativeList(List<GameUnit> unitList, int initiativeHealthBarHeight,
            IRenderable[,] unitListGrid)
        {
            IRenderable[,] firstUnitContent =
            {
                {
                    new RenderText(GameDriver.MapFont, unitList[0].Id)
                },
                {
                    unitList[0].MediumPortrait
                },
                {
                    unitList[0]
                        .GetInitiativeHealthBar(new Vector2(unitList[0].MediumPortrait.Width,
                            initiativeHealthBarHeight))
                }
            };

            IRenderable firstSingleUnitContent = new Window("Unit", windowTexture,
                new WindowContentGrid(firstUnitContent, 2),
                new Color(100, 200, 100, 225));
            unitListGrid[0, 0] = firstSingleUnitContent;
        }

        private void GenerateRestOfInitiativeList(List<GameUnit> unitList, IRenderable[,] unitListGrid,
            int initiativeHealthBarHeight)
        {
            for (int i = 1; i < unitListGrid.GetLength(1); i++)
            {
                IRenderable[,] unitContent =
                {
                    {
                        new RenderText(GameDriver.MapFont, unitList[i].Id)
                    },
                    {
                        unitList[i].MediumPortrait
                    },
                    {
                        unitList[i]
                            .GetInitiativeHealthBar(new Vector2(unitList[i].MediumPortrait.Width,
                                initiativeHealthBarHeight))
                    }
                };

                IRenderable singleUnitContent = new Window("Unit", windowTexture,
                    new WindowContentGrid(unitContent, 2),
                    ColorMapper.DetermineTeamColor(unitList[i].UnitTeam));
                unitListGrid[0, i] = singleUnitContent;
            }
        }

        public void UpdateLeftPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            GenerateUnitPortraitWindow(hoverMapUnit, ref leftUnitPortraitWindow);
            GenerateUnitDetailWindow(hoverMapUnit, ref leftUnitDetailWindow);
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            GenerateUnitPortraitWindow(hoverMapUnit, ref rightUnitPortraitWindow);
            GenerateUnitDetailWindow(hoverMapUnit, ref rightUnitDetailWindow);
        }

        // ReSharper disable once RedundantAssignment
        private void GenerateUnitPortraitWindow(GameUnit selectedUnit, ref Window windowToUpdate)
        {
            if (selectedUnit == null)
            {
                windowToUpdate = null;
                return;
            }

            const int hoverWindowHealthBarHeight = 15;
            IRenderable[,] selectedUnitPortrait =
            {
                {
                    selectedUnit.LargePortrait
                },
                {
                    selectedUnit.GetHoverWindowHealthBar(new Vector2(selectedUnit.LargePortrait.Width,
                        hoverWindowHealthBarHeight))
                }
            };

            string windowLabel = "Selected Portrait: " + selectedUnit.Id;

            Color windowColour = ColorMapper.DetermineTeamColor(selectedUnit.UnitTeam);

            windowToUpdate = new Window(windowLabel, windowTexture, new WindowContentGrid(selectedUnitPortrait, 1),
                windowColour);
        }

        // ReSharper disable once RedundantAssignment
        private void GenerateUnitDetailWindow(GameUnit selectedUnit, ref Window windowToUpdate)
        {
            if (selectedUnit == null)
            {
                windowToUpdate = null;
                return;
            }

            IRenderable selectedUnitInfo =
                new RenderText(GameDriver.WindowFont,
                    selectedUnit.Id + "\n" + selectedUnit.UnitTeam + " " + selectedUnit.UnitJobClass + "\n" +
                    selectedUnit.Stats);

            string windowLabel = "Selected Info: " + selectedUnit.Id;

            Color windowColour = ColorMapper.DetermineTeamColor(selectedUnit.UnitTeam);

            windowToUpdate = new Window(windowLabel, windowTexture, selectedUnitInfo, windowColour);
        }


        #region Window Positions

        private Vector2 LeftUnitPortraitWindowPosition()
        {
            //Bottom-left, above initiative window
            return new Vector2(WindowEdgeBuffer,
                screenSize.Y - LeftUnitPortraitWindow.Height - InitiativeWindow.Height);
        }

        private Vector2 LeftUnitDetailWindowPosition()
        {
            //Bottom-left, right of portrait, above initiative window
            return new Vector2(WindowEdgeBuffer + LeftUnitPortraitWindow.Width,
                LeftUnitPortraitWindowPosition().Y + LeftUnitPortraitWindow.Height - LeftUnitDetailWindow.Height -
                UnitDetailVerticalAdjustment
            );
        }

        private Vector2 RightUnitPortraitWindowPosition()
        {
            //Bottom-right, above intiative window
            return new Vector2(screenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                screenSize.Y - RightUnitPortraitWindow.Height - InitiativeWindow.Height);
        }

        private Vector2 RightUnitDetailWindowPosition()
        {
            //Bottom-right, left of portrait, above intiative window
            return new Vector2(
                screenSize.X - RightUnitDetailWindow.Width - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                RightUnitPortraitWindowPosition().Y + RightUnitPortraitWindow.Height - RightUnitDetailWindow.Height -
                UnitDetailVerticalAdjustment
            );
        }

        private Vector2 InitiativeWindowPosition()
        {
            //Bottom-right
            return new Vector2(screenSize.X - InitiativeWindow.Width - WindowEdgeBuffer,
                screenSize.Y - InitiativeWindow.Height);
        }

        private Vector2 TurnWindowPosition()
        {
            //Bottom-right
            return new Vector2(WindowEdgeBuffer, screenSize.Y - TurnWindow.Height);
        }

        private Vector2 TerrainWindowPosition()
        {
            //Top-right
            return new Vector2(screenSize.X - TerrainEntityWindow.Width - WindowEdgeBuffer, WindowEdgeBuffer);
        }

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return new Vector2(WindowEdgeBuffer);
        }

        private Vector2 UserPromptWindowPosition()
        {
            //Middle of the screen
            return new Vector2(GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) UserPromptWindow.Height / 2);
        }

        #endregion Window Positions

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());
            }

            if (TerrainEntityWindow != null)
            {
                TerrainEntityWindow.Draw(spriteBatch, TerrainWindowPosition());
            }

            if (TurnWindow != null)
            {
                TurnWindow.Draw(spriteBatch, TurnWindowPosition());
            }

            if (InitiativeWindow != null)
            {
                InitiativeWindow.Draw(spriteBatch, InitiativeWindowPosition());

                if (LeftUnitPortraitWindow != null)
                {
                    LeftUnitPortraitWindow.Draw(spriteBatch, LeftUnitPortraitWindowPosition());

                    if (LeftUnitDetailWindow != null)
                    {
                        LeftUnitDetailWindow.Draw(spriteBatch, LeftUnitDetailWindowPosition());
                    }
                }

                if (RightUnitPortraitWindow != null)
                {
                    RightUnitPortraitWindow.Draw(spriteBatch, RightUnitPortraitWindowPosition());

                    if (RightUnitDetailWindow != null)
                    {
                        RightUnitDetailWindow.Draw(spriteBatch, RightUnitDetailWindowPosition());
                    }
                }

                if (UserPromptWindow != null)
                {
                    UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
                }
            }
        }
    }
}