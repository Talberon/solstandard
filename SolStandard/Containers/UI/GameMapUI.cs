using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    /*
     * GameMapUI is where the HUD elements for the SelectMapEntity Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class GameMapUI : IUserInterface
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

        public VerticalMenu ActionMenu { get; private set; }

        private bool visible;

        private readonly ITexture2D windowTexture;

        public GameMapUI(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            windowTexture = AssetManager.WindowTexture;
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

        #region Generation

        public void GenerateCombatMenu()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);
            int skillCount = GameContext.ActiveUnit.Skills.Count;

            MenuOption[] options = new MenuOption[skillCount];

            for (int i = 0; i < skillCount; i++)
            {
                options[i] = new SkillOption(windowColour, GameContext.ActiveUnit.Skills[i]);
            }

            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height), 1);


            ActionMenu = new VerticalMenu(options, cursorSprite, windowColour);
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window("User Prompt Window", windowTexture, promptTextContent, promptWindowColor,
                sizeOverride);
        }

        public void GenerateTurnWindow(Vector2 windowSize)
        {
            string turnInfo = "Turn: " + GameContext.TurnNumber;
            turnInfo += "\n";
            turnInfo += "Active Team: " + GameContext.ActiveUnit.Team;
            turnInfo += "\n";
            turnInfo += "Active Unit: " + GameContext.ActiveUnit.Id;

            WindowContentGrid unitListContentGrid = new WindowContentGrid(
                new[,]
                {
                    {
                        GameContext.ActiveUnit.GetMapSprite(new Vector2(64)),
                        new RenderText(AssetManager.WindowFont, turnInfo)
                    }
                },
                1
            );

            TurnWindow = new Window("Turn Counter", windowTexture, unitListContentGrid, new Color(100, 100, 100, 225),
                windowSize);
        }

        public void GenerateTerrainWindow(TerrainEntity selectedTerrain)
        {
            WindowContentGrid terrainContentGrid;

            if (selectedTerrain != null)
            {
                terrainContentGrid = new WindowContentGrid(
                    new[,]
                    {
                        {
                            selectedTerrain.TerrainInfo
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
                            new RenderText(AssetManager.WindowFont, "None ")
                        }
                    },
                    1);
            }

            TerrainEntityWindow = new Window("Terrain Info", windowTexture, terrainContentGrid,
                new Color(100, 150, 100, 220));
        }

        public void GenerateHelpWindow(string helpText)
        {
            IRenderable textToRender = new RenderText(AssetManager.WindowFont, helpText);
            HelpTextWindow = new Window("Help Text", windowTexture, textToRender, new Color(30, 30, 30, 150));
        }

        public void GenerateInitiativeWindow(List<GameUnit> unitList)
        {
            //TODO figure out if we really want this to be hard-coded or determined based on screen size or something
            const int maxInitiativeSize = 9;

            int initiativeListLength = (unitList.Count > maxInitiativeSize) ? maxInitiativeSize : unitList.Count;

            IRenderable[,] unitListGrid = new IRenderable[1, initiativeListLength];
            const int initiativeHealthBarHeight = 10;

            GenerateFirstUnitInInitiativeList(unitList, initiativeHealthBarHeight, unitListGrid);
            GenerateRestOfInitiativeList(unitList, unitListGrid, initiativeHealthBarHeight);


            WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListGrid, 3);

            InitiativeWindow = new Window(
                "Initiative List",
                windowTexture,
                unitListContentGrid,
                new Color(100, 100, 100, 225)
            );
        }

        private void GenerateFirstUnitInInitiativeList(List<GameUnit> unitList, int initiativeHealthBarHeight,
            IRenderable[,] unitListGrid)
        {
            IRenderable[,] firstUnitContent =
            {
                {
                    new RenderText(AssetManager.MapFont, unitList[0].Id)
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

            IRenderable firstSingleUnitContent = new Window(
                "First Unit " + unitList[0].Id + " Window",
                windowTexture,
                new WindowContentGrid(firstUnitContent, 2),
                new Color(100, 200, 100, 225)
            );
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
                        new RenderText(AssetManager.MapFont, unitList[i].Id)
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

                IRenderable singleUnitContent = new Window(
                    "Unit " + unitList[i].Id + " Window",
                    windowTexture,
                    new WindowContentGrid(unitContent, 2),
                    TeamUtility.DetermineTeamColor(unitList[i].Team)
                );
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

            Color windowColour = TeamUtility.DetermineTeamColor(selectedUnit.Team);

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

            IRenderable selectedUnitInfo = selectedUnit.DetailPane;

            string windowLabel = "Selected Info: " + selectedUnit.Id;

            Color windowColour = TeamUtility.DetermineTeamColor(selectedUnit.Team);

            windowToUpdate = new Window(windowLabel, windowTexture, selectedUnitInfo, windowColour);
        }

        #endregion Generation


        #region Window Positions

        private Vector2 CombatMenuPosition()
        {
            return new Vector2(
                WindowEdgeBuffer + LeftUnitDetailWindowPosition().X + LeftUnitDetailWindow.Width,
                LeftUnitDetailWindowPosition().Y
            );
        }

        private Vector2 LeftUnitPortraitWindowPosition()
        {
            //Bottom-left, above initiative window
            return new Vector2(WindowEdgeBuffer,
                screenSize.Y - LeftUnitPortraitWindow.Height - InitiativeWindow.Height
            );
        }

        private Vector2 LeftUnitDetailWindowPosition()
        {
            //Bottom-left, right of portrait, above initiative window
            return new Vector2(
                WindowEdgeBuffer + LeftUnitPortraitWindow.Width,
                LeftUnitPortraitWindowPosition().Y + LeftUnitPortraitWindow.Height - LeftUnitDetailWindow.Height -
                UnitDetailVerticalAdjustment
            );
        }

        private Vector2 RightUnitPortraitWindowPosition()
        {
            //Bottom-right, above intiative window
            return new Vector2(
                screenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                screenSize.Y - RightUnitPortraitWindow.Height - InitiativeWindow.Height
            );
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
            return new Vector2(
                screenSize.X - InitiativeWindow.Width - WindowEdgeBuffer,
                screenSize.Y - InitiativeWindow.Height
            );
        }

        private Vector2 TurnWindowPosition()
        {
            //Bottom-right
            return new Vector2(
                WindowEdgeBuffer,
                screenSize.Y - TurnWindow.Height
            );
        }

        private Vector2 TerrainWindowPosition()
        {
            //Top-right
            return new Vector2(
                screenSize.X - TerrainEntityWindow.Width - WindowEdgeBuffer,
                WindowEdgeBuffer
            );
        }

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return new Vector2(WindowEdgeBuffer);
        }

        private Vector2 UserPromptWindowPosition()
        {
            //Middle of the screen
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) UserPromptWindow.Height / 2
            );
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

            if (ActionMenu != null)
            {
                ActionMenu.Draw(spriteBatch, CombatMenuPosition());
            }
        }
    }
}