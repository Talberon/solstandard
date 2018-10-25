using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Containers.View;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.Controller
{
    public class GameMapController
    {
        private GameMapView GameMapView { get; set; }

        public GameMapController()
        {
            GameMapView = new GameMapView();
        }

        public VerticalMenu ActionMenu
        {
            get { return GameMapView.ActionMenu; }
        }

        public void ClearCombatMenu()
        {
            GameMapView.ActionMenu = null;
        }

        public void ClosePromptWindow()
        {
            GameMapView.UserPromptWindow = null;
        }

        public void GenerateActionMenu()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);

            MenuOption[] options = UnitContextualActionMenuContext.GenerateActionMenuOptions(windowColour);

            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height), 1);

            GameMapView.ActionMenu = new VerticalMenu(options, cursorSprite, windowColour);
            GenerateActionMenuDescription();
        }

        public void GenerateActionMenuDescription()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);
            GameMapView.ActionMenuDescriptionWindow = new Window(
                "Action Menu Description",
                AssetManager.WindowTexture,
                new RenderText(
                    AssetManager.WindowFont,
                    UnitContextualActionMenuContext.GetActionDescriptionAtIndex(GameMapView.ActionMenu.CurrentOptionIndex)
                ),
                windowColour
            );
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            GameMapView.UserPromptWindow = new Window("User Prompt Window", AssetManager.WindowTexture, promptTextContent,
                promptWindowColor,
                sizeOverride);
        }

        public void GenerateTurnWindow()
        {
            //FIXME Stop hardcoding the X-Value of the Turn Window
            Vector2 turnWindowSize = new Vector2(300, GameMapView.InitiativeWindow.Height);

            string turnInfo = "Turn: " + GameContext.TurnCounter;
            turnInfo += Environment.NewLine;
            turnInfo += "Round: " + GameContext.RoundCounter;
            turnInfo += Environment.NewLine;
            turnInfo += "Active Team: " + GameContext.ActiveUnit.Team;
            turnInfo += Environment.NewLine;
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

            GameMapView.TurnWindow = new Window("Turn Counter", AssetManager.WindowTexture, unitListContentGrid,
                new Color(100, 100, 100, 225), turnWindowSize);
        }

        public void GenerateEntityWindow(MapSlice hoverSlice)
        {
            WindowContentGrid terrainContentGrid;

            if (hoverSlice.TerrainEntity != null || hoverSlice.ItemEntity != null)
            {
                terrainContentGrid = new WindowContentGrid(
                    new[,]
                    {
                        {
                            (hoverSlice.TerrainEntity != null)
                                ? new Window(
                                    "Terrain Preview",
                                    AssetManager.WindowTexture,
                                    hoverSlice.TerrainEntity.TerrainInfo,
                                    new Color(100, 150, 100, 180)
                                ) as IRenderable
                                : new RenderBlank()
                        },
                        {
                            (hoverSlice.ItemEntity != null)
                                ? new Window(
                                    "Item Preview",
                                    AssetManager.WindowTexture,
                                    hoverSlice.ItemEntity.TerrainInfo,
                                    new Color(180, 180, 100, 180)
                                ) as IRenderable
                                : new RenderBlank()
                        }
                    },
                    1);
            }
            else
            {
                bool canMove = UnitMovingContext.CanMoveAtCoordinates(hoverSlice.MapCoordinates);

                WindowContentGrid noEntityContent = new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, "None"),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(StatIcons.Mv),
                            new RenderText(AssetManager.WindowFont, (canMove) ? "Can Move" : "No Move",
                                (canMove) ? TerrainEntity.PositiveColor : TerrainEntity.NegativeColor)
                        }
                    },
                    1
                );

                terrainContentGrid = new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new Window(
                                "No Entity Info",
                                AssetManager.WindowTexture,
                                noEntityContent,
                                new Color(100, 100, 100, 180)
                            )
                        }
                    },
                    1);
            }

            GameMapView.EntityWindow = new Window("Entity Info", AssetManager.WindowTexture, terrainContentGrid,
                new Color(50, 50, 50, 150));
        }

        public void GenerateHelpWindow(string helpText)
        {
            Color helpWindowColor = new Color(30, 30, 30, 150);

            IRenderable textToRender = new RenderText(AssetManager.WindowFont, helpText);


            WindowContentGrid helpWindowContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {new Window("HelpText", AssetManager.WindowTexture, textToRender, helpWindowColor)},
                },
                1
            );

            GameMapView.HelpTextWindow = new Window("Help Text", AssetManager.WindowTexture, helpWindowContentGrid,
                Color.Transparent);
        }

        public void GenerateObjectiveWindow()
        {
            GameMapView.ObjectiveWindow = GameContext.Scenario.ScenarioInfo;
        }

        public void GenerateInitiativeWindow(List<GameUnit> unitList)
        {
            //TODO figure out if we really want this to be hard-coded or determined based on screen size or something
            const int maxInitiativeSize = 16;

            int initiativeListLength = (unitList.Count > maxInitiativeSize) ? maxInitiativeSize : unitList.Count;

            IRenderable[,] unitListGrid = new IRenderable[1, initiativeListLength];
            const int initiativeHealthBarHeight = 10;

            GenerateFirstUnitInInitiativeList(unitList, initiativeHealthBarHeight, unitListGrid);
            GenerateRestOfInitiativeList(unitList, unitListGrid, initiativeHealthBarHeight);


            WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListGrid, 3);

            GameMapView.InitiativeWindow = new Window(
                "Initiative List",
                AssetManager.WindowTexture,
                unitListContentGrid,
                new Color(100, 100, 100, 225)
            );
        }

        private void GenerateFirstUnitInInitiativeList(List<GameUnit> unitList, int initiativeHealthBarHeight,
            IRenderable[,] unitListGrid)
        {
            IRenderable firstSingleUnitContent = SingleUnitContent(unitList[0], initiativeHealthBarHeight);
            unitListGrid[0, 0] = firstSingleUnitContent;
        }

        private static void GenerateRestOfInitiativeList(List<GameUnit> unitList, IRenderable[,] unitListGrid,
            int initiativeHealthBarHeight)
        {
            for (int i = 1; i < unitListGrid.GetLength(1); i++)
            {
                IRenderable singleUnitContent = SingleUnitContent(unitList[i], initiativeHealthBarHeight);
                unitListGrid[0, i] = singleUnitContent;
            }
        }

        private static IRenderable SingleUnitContent(GameUnit unit, int initiativeHealthBarHeight)
        {
            IRenderable[,] unitContent =
            {
                {
                    new RenderText(AssetManager.MapFont, unit.Id)
                },
                {
                    unit.SmallPortrait
                },
                {
                    unit.GetInitiativeHealthBar(new Vector2(unit.SmallPortrait.Width, initiativeHealthBarHeight))
                }
            };

            IRenderable singleUnitContent = new Window(
                "Unit " + unit.Id + " Window",
                AssetManager.WindowTexture,
                new WindowContentGrid(unitContent, 3, HorizontalAlignment.Centered),
                TeamUtility.DetermineTeamColor(unit.Team)
            );
            return singleUnitContent;
        }

        public void UpdateLeftPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            GameMapView.LeftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            GameMapView.LeftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            GameMapView.LeftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
            GameMapView.LeftUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit);
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            GameMapView.RightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            GameMapView.RightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            GameMapView.RightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
            GameMapView.RightUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit);
        }

        private static Window GenerateUnitPortraitWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            string windowLabel = "Selected Portrait: " + selectedUnit.Id;
            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(windowLabel, AssetManager.WindowTexture, selectedUnit.UnitPortraitPane, windowColor);
        }

        private Window GenerateUnitDetailWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            IRenderable selectedUnitInfo = selectedUnit.DetailPane;

            string windowLabel = "Selected Info: " + selectedUnit.Id;
            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(windowLabel, AssetManager.WindowTexture, selectedUnitInfo, windowColor);
        }

        private static Window GenerateUnitInventoryWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null || selectedUnit.InventoryPane == null) return null;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window("Unit Inventory: " + selectedUnit.Id, AssetManager.WindowTexture,
                selectedUnit.InventoryPane, windowColor);
        }

        private static Window GenerateUnitStatusWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null || selectedUnit.StatusEffects.Count < 1) return null;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);

            IRenderable[,] selectedUnitStatuses = new IRenderable[selectedUnit.StatusEffects.Count, 1];

            for (int i = 0; i < selectedUnit.StatusEffects.Count; i++)
            {
                StatusEffect effect = selectedUnit.StatusEffects[i];

                selectedUnitStatuses[i, 0] = new Window(
                    "Status " + effect.Name,
                    AssetManager.WindowTexture,
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
            return new Window(windowLabel, AssetManager.WindowTexture, new WindowContentGrid(selectedUnitStatuses, 1),
                windowColor);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GameMapView.Draw(spriteBatch);
        }
    }
}