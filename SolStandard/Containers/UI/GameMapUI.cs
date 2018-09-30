﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
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

        public Window LeftUnitPortraitWindow { get; private set; }
        public Window LeftUnitDetailWindow { get; private set; }
        public Window LeftUnitStatusWindow { get; private set; }

        public Window RightUnitPortraitWindow { get; private set; }
        public Window RightUnitDetailWindow { get; private set; }
        public Window RightUnitStatusWindow { get; private set; }

        public Window TurnWindow { get; private set; }
        public Window InitiativeWindow { get; private set; }
        public Window TerrainEntityWindow { get; private set; }
        public Window HelpTextWindow { get; private set; }

        public Window UserPromptWindow { get; private set; }

        public VerticalMenu ActionMenu { get; private set; }
        public Window ActionMenuDescriptionWindow { get; private set; }

        private bool visible;

        private readonly ITexture2D windowTexture;

        public GameMapUI(Vector2 screenSize)
        {
            this.screenSize = screenSize;
            windowTexture = AssetManager.WindowTexture;
            visible = true;
        }

        public void ClearCombatMenu()
        {
            ActionMenu = null;
        }

        #region Generation

        public void GenerateActionMenu()
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

            GenerateActionMenuDescription();
        }

        public void GenerateActionMenuDescription()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);
            ActionMenuDescriptionWindow = new Window(
                "Action Menu Description",
                windowTexture,
                new RenderText(
                    AssetManager.WindowFont,
                    GameContext.ActiveUnit.Skills[ActionMenu.CurrentOptionIndex].Description
                ),
                windowColour
            );
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
                        GameContext.ActiveUnit.GetMapSprite(new Vector2(100)),
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
            LeftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            LeftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            LeftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            RightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            RightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            RightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
        }

        private Window GenerateUnitPortraitWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

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
            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(windowLabel, windowTexture, new WindowContentGrid(selectedUnitPortrait, 1), windowColor);
        }

        private Window GenerateUnitDetailWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            IRenderable selectedUnitInfo = selectedUnit.DetailPane;

            string windowLabel = "Selected Info: " + selectedUnit.Id;
            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(windowLabel, windowTexture, selectedUnitInfo, windowColor);
        }

        private Window GenerateUnitStatusWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null || selectedUnit.StatusEffects.Count < 1) return null;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);

            IRenderable[,] selectedUnitStatuses = new IRenderable[1, selectedUnit.StatusEffects.Count];

            for (int i = 0; i < selectedUnit.StatusEffects.Count; i++)
            {
                StatusEffect effect = selectedUnit.StatusEffects[i];

                selectedUnitStatuses[0, i] = new Window(
                    "Status " + effect.Name,
                    windowTexture,
                    new WindowContentGrid(
                        new[,]
                        {
                            {
                                new RenderText(AssetManager.WindowFont, "[" + effect.TurnDuration + "]"),
                                effect.StatusIcon,
                                new RenderText(AssetManager.WindowFont, effect.Name)
                            }
                        },
                        1
                    ),
                    windowColor
                );
            }

            string windowLabel = "Selected Status: " + selectedUnit.Id;
            return new Window(windowLabel, windowTexture, new WindowContentGrid(selectedUnitStatuses, 1), windowColor);
        }

        #endregion Generation


        #region Window Positions

        private Vector2 ActionMenuPosition()
        {
            return new Vector2(
                WindowEdgeBuffer + LeftUnitDetailWindowPosition().X + LeftUnitDetailWindow.Width,
                LeftUnitDetailWindowPosition().Y
            );
        }

        private Vector2 ActionMenuDescriptionPosition()
        {
            return new Vector2(
                WindowEdgeBuffer + ActionMenuPosition().X + ActionMenu.Width,
                ActionMenuPosition().Y
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
                LeftUnitPortraitWindowPosition().Y + LeftUnitPortraitWindow.Height - LeftUnitDetailWindow.Height
            );
        }

        private Vector2 LeftUnitStatusWindowPosition()
        {
            //Bottom-left, above portrait
            return new Vector2(
                LeftUnitPortraitWindowPosition().X,
                LeftUnitPortraitWindowPosition().Y - LeftUnitStatusWindow.Height - WindowEdgeBuffer
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
                RightUnitPortraitWindowPosition().Y + RightUnitPortraitWindow.Height - RightUnitDetailWindow.Height
            );
        }


        private Vector2 RightUnitStatusWindowPosition()
        {
            //Bottom-right, above portrait
            return new Vector2(
                RightUnitPortraitWindowPosition().X + RightUnitPortraitWindow.Width - RightUnitStatusWindow.Width,
                RightUnitPortraitWindowPosition().Y - RightUnitStatusWindow.Height - WindowEdgeBuffer
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

                    if (LeftUnitStatusWindow != null)
                    {
                        LeftUnitStatusWindow.Draw(spriteBatch, LeftUnitStatusWindowPosition());
                    }
                }

                if (RightUnitPortraitWindow != null)
                {
                    RightUnitPortraitWindow.Draw(spriteBatch, RightUnitPortraitWindowPosition());

                    if (RightUnitDetailWindow != null)
                    {
                        RightUnitDetailWindow.Draw(spriteBatch, RightUnitDetailWindowPosition());
                    }

                    if (RightUnitStatusWindow != null)
                    {
                        RightUnitStatusWindow.Draw(spriteBatch, RightUnitStatusWindowPosition());
                    }
                }

                if (UserPromptWindow != null)
                {
                    UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
                }
            }

            if (ActionMenu != null)
            {
                ActionMenu.Draw(spriteBatch, ActionMenuPosition());

                if (ActionMenuDescriptionWindow != null)
                {
                    ActionMenuDescriptionWindow.Draw(spriteBatch, ActionMenuDescriptionPosition());
                }
            }
        }
    }
}