using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
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

namespace SolStandard.Containers.View
{
    /*
     * GameMapContext is where the HUD elements for the SelectMapEntity Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class GameMapView : IUserInterface
    {
        public enum MenuType
        {
            ActionMenu,
            InventoryMenu
        }

        private const int WindowEdgeBuffer = 5;
        private static readonly Color TeamListWindowBackgroundColor = new Color(100, 100, 100, 225);

        private Window LeftUnitPortraitWindow { get; set; }
        private Window LeftUnitDetailWindow { get; set; }
        private Window LeftUnitStatusWindow { get; set; }
        private Window LeftUnitInventoryWindow { get; set; }

        private Window RightUnitPortraitWindow { get; set; }
        private Window RightUnitDetailWindow { get; set; }
        private Window RightUnitStatusWindow { get; set; }
        private Window RightUnitInventoryWindow { get; set; }

        private Window InitiativeWindow { get; set; }
        private Window BlueTeamWindow { get; set; }
        private Window RedTeamWindow { get; set; }
        private Window EntityWindow { get; set; }
        private Window ObjectiveWindow { get; set; }

        private Window UserPromptWindow { get; set; }

        private VerticalMenu ActionMenu { get; set; }
        private Window ActionMenuDescriptionWindow { get; set; }

        private VerticalMenu InventoryMenu { get; set; }
        private Window InventoryMenuDescriptionWindow { get; set; }

        private Window MenuDescriptionWindow { get; set; }

        private MenuType visibleMenu;
        private bool visible;

        public GameMapView()
        {
            visible = true;
        }

        private MenuType VisibleMenu
        {
            get { return visibleMenu; }
            set
            {
                visibleMenu = value;
                switch (value)
                {
                    case MenuType.ActionMenu:
                        ActionMenu.IsVisible = true;
                        InventoryMenu.IsVisible = false;
                        break;
                    case MenuType.InventoryMenu:
                        ActionMenu.IsVisible = false;
                        InventoryMenu.IsVisible = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value", value, null);
                }
            }
        }

        public VerticalMenu CurrentMenu
        {
            get
            {
                switch (VisibleMenu)
                {
                    case MenuType.ActionMenu:
                        return ActionMenu;
                    case MenuType.InventoryMenu:
                        return InventoryMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void ToggleCombatMenu()
        {
            if (VisibleMenu == MenuType.ActionMenu)
            {
                VisibleMenu = MenuType.InventoryMenu;
            }
            else if (VisibleMenu == MenuType.InventoryMenu)
            {
                VisibleMenu = MenuType.ActionMenu;
            }

            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);
            GenerateMenuDescriptionWindow(VisibleMenu, windowColour);
        }

        #region Close Windows

        public void CloseUserPromptWindow()
        {
            UserPromptWindow = null;
        }

        public void CloseCombatMenu()
        {
            ActionMenu.IsVisible = false;
            InventoryMenu.IsVisible = false;
        }

        #endregion Close Windows

        #region Generation

        public void GenerateActionMenus()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);
            GenerateActionMenu(windowColour);
            GenerateInventoryMenu(windowColour);
            VisibleMenu = MenuType.ActionMenu;
            GenerateMenuDescriptionWindow(VisibleMenu, windowColour);
        }

        private void GenerateMenuDescriptionWindow(MenuType menuType, Color windowColor)
        {
            string menuName;
            IRenderable buttonIcon;
            RenderText windowText;

            switch (menuType)
            {
                case MenuType.ActionMenu:
                    menuName = "Unit Actions";
                    windowText = new RenderText(AssetManager.HeaderFont, menuName);
                    buttonIcon = ButtonIconProvider.GetButton(ButtonIcon.DpadRight, new Vector2(windowText.Height));
                    break;
                case MenuType.InventoryMenu:
                    menuName = "Inventory";
                    windowText = new RenderText(AssetManager.HeaderFont, menuName);
                    buttonIcon = ButtonIconProvider.GetButton(ButtonIcon.DpadLeft, new Vector2(windowText.Height));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("menuType", menuType, null);
            }


            MenuDescriptionWindow = new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            buttonIcon,
                            windowText
                        }
                    },
                    3,
                    HorizontalAlignment.Centered
                ),
                windowColor,
                HorizontalAlignment.Centered
            );
        }

        private void GenerateActionMenu(Color windowColor)
        {
            MenuOption[] options = UnitContextualActionMenuContext.GenerateActionMenuOptions(windowColor);

            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            ActionMenu = new VerticalMenu(options, cursorSprite, windowColor);
        }

        private void GenerateInventoryMenu(Color windowColor)
        {
            MenuOption[] options = UnitContextualActionMenuContext.GenerateInventoryMenuOptions(windowColor);

            IRenderable cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));

            InventoryMenu = new VerticalMenu(options, cursorSprite, windowColor);
        }

        public void GenerateCurrentMenuDescription()
        {
            Color windowColour = TeamUtility.DetermineTeamColor(GameContext.ActiveUnit.Team);

            switch (VisibleMenu)
            {
                case MenuType.ActionMenu:
                    GenerateActionMenuDescription(windowColour);
                    break;
                case MenuType.InventoryMenu:
                    GenerateInventoryMenuDescription(windowColour);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateActionMenuDescription(Color windowColor)
        {
            ActionMenuDescriptionWindow = new Window(
                new RenderText(
                    AssetManager.WindowFont,
                    UnitContextualActionMenuContext.GetActionDescriptionAtIndex(ActionMenu)
                ),
                windowColor
            );
        }

        private void GenerateInventoryMenuDescription(Color windowColor)
        {
            InventoryMenuDescriptionWindow = new Window(
                new RenderText(
                    AssetManager.WindowFont,
                    UnitContextualActionMenuContext.GetActionDescriptionAtIndex(InventoryMenu)
                ),
                windowColor
            );
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window(
                promptTextContent,
                promptWindowColor,
                sizeOverride
            );
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
                                    hoverSlice.TerrainEntity.TerrainInfo,
                                    new Color(100, 150, 100, 180)
                                ) as IRenderable
                                : new RenderBlank()
                        },
                        {
                            (hoverSlice.ItemEntity != null)
                                ? new Window(
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
                bool canMove = UnitMovingContext.CanEndMoveAtCoordinates(hoverSlice.MapCoordinates);

                WindowContentGrid noEntityContent = new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont, "None"),
                            new RenderBlank()
                        },
                        {
                            UnitStatistics.GetSpriteAtlas(Stats.Mv),
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
                                noEntityContent,
                                new Color(100, 100, 100, 180)
                            )
                        }
                    },
                    1);
            }

            EntityWindow = new Window(terrainContentGrid,
                new Color(50, 50, 50, 150));
        }

        public void GenerateObjectiveWindow()
        {
            ObjectiveWindow = GameContext.Scenario.ScenarioInfo;
        }

        public void GenerateInitiativeWindow()
        {
            GenerateTeamInitiativeWindow(Team.Blue);
            GenerateTeamInitiativeWindow(Team.Red);

            InitiativeWindow = new Window(
                new WindowContentGrid(new IRenderable[,] {{BlueTeamWindow, RedTeamWindow}}, 1),
                Color.Transparent,
                HorizontalAlignment.Centered
            );
        }

        private void GenerateTeamInitiativeWindow(Team team)
        {
            //TODO figure out if we really want this to be hard-coded or determined based on screen size or something
            const int maxInitiativeSize = 12;

            List<GameUnit> unitList = GameContext.Units.FindAll(unit => unit.Team == team);


            int initiativeListLength = (unitList.Count > maxInitiativeSize) ? maxInitiativeSize : unitList.Count;

            IRenderable[,] unitListContent = new IRenderable[1, initiativeListLength];
            const int initiativeHealthBarHeight = 10;

            GenerateInitiativeList(unitList, unitListContent, initiativeHealthBarHeight);

            WindowContentGrid unitListContentGrid = new WindowContentGrid(unitListContent, 0);

            switch (team)
            {
                case Team.Blue:
                    BlueTeamWindow = new Window(unitListContentGrid, TeamListWindowBackgroundColor);
                    break;
                case Team.Red:
                    RedTeamWindow = new Window(unitListContentGrid, TeamListWindowBackgroundColor);
                    break;
                case Team.Creep:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }

        private static void GenerateInitiativeList(IReadOnlyList<GameUnit> unitList, IRenderable[,] unitListGrid,
            int initiativeHealthBarHeight)
        {
            for (int i = 0; i < unitListGrid.GetLength(1); i++)
            {
                IRenderable singleUnitContent = SingleUnitContent(unitList[i], initiativeHealthBarHeight);
                unitListGrid[0, i] = singleUnitContent;
            }
        }

        public static IRenderable SingleUnitContent(GameUnit unit, int initiativeHealthBarHeight)
        {
            const int crownIconSize = 12;

            IRenderable[,] unitContent =
            {
                {
                    unit.IsCommander
                        ? GameUnit.GetCommanderCrown(new Vector2(crownIconSize))
                        : new RenderBlank() as IRenderable,
                    new RenderText(AssetManager.MapFont, unit.Id)
                },
                {
                    new RenderBlank(),
                    unit.SmallPortrait
                },
                {
                    new RenderBlank(),
                    unit.GetInitiativeHealthBar(new Vector2(unit.SmallPortrait.Width, initiativeHealthBarHeight))
                }
            };

            IRenderable singleUnitContent = new Window(
                new WindowContentGrid(unitContent, 3, HorizontalAlignment.Centered),
                TeamUtility.DetermineTeamColor(unit.Team)
            );
            return singleUnitContent;
        }

        public void UpdateLeftPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            LeftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            LeftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            LeftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
            LeftUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit);
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            RightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit);
            RightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit);
            RightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit);
            RightUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit);
        }

        private static Window GenerateUnitPortraitWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(selectedUnit.UnitPortraitPane, windowColor);
        }

        private Window GenerateUnitDetailWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null) return null;

            IRenderable selectedUnitInfo = selectedUnit.DetailPane;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(selectedUnitInfo, windowColor);
        }

        private static Window GenerateUnitInventoryWindow(GameUnit selectedUnit)
        {
            if (selectedUnit == null || selectedUnit.InventoryPane == null) return null;

            Color windowColor = TeamUtility.DetermineTeamColor(selectedUnit.Team);
            return new Window(selectedUnit.InventoryPane, windowColor);
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
                    new WindowContentGrid(
                        new[,]
                        {
                            {
                                new RenderText(AssetManager.WindowFont, "[" + effect.TurnDuration + "T]"),
                                effect.StatusIcon,
                                new RenderText(AssetManager.WindowFont, effect.Name)
                            }
                        },
                        1
                    ),
                    windowColor
                );
            }

            return new Window(new WindowContentGrid(selectedUnitStatuses, 1), windowColor);
        }

        #endregion Generation


        #region Window Positions

        private Vector2 ActionMenuPosition()
        {
            //Center of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 3 - ActionMenu.Width,
                (GameDriver.ScreenSize.Y / 2) - ((float) ActionMenu.Height / 2)
            );
        }

        private Vector2 InventoryMenuPosition()
        {
            //Center of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 3 - InventoryMenu.Width,
                (GameDriver.ScreenSize.Y / 2) - ((float) InventoryMenu.Height / 2)
            );
        }

        private Vector2 ActionMenuDescriptionPosition()
        {
            //Right of Action Menu
            return new Vector2(
                WindowEdgeBuffer + ActionMenuPosition().X + ActionMenu.Width,
                ActionMenuPosition().Y
            );
        }

        private Vector2 InventoryMenuDescriptionPosition()
        {
            //Right of Action Menu
            return new Vector2(
                WindowEdgeBuffer + InventoryMenuPosition().X + InventoryMenu.Width,
                InventoryMenuPosition().Y
            );
        }

        private Vector2 LeftUnitPortraitWindowPosition()
        {
            //Bottom-left, above initiative window
            return new Vector2(WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - LeftUnitPortraitWindow.Height - BlueTeamWindow.Height
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


        private Vector2 LeftUnitInventoryWindowPosition()
        {
            //Bottom-left, right of stats
            return new Vector2(
                LeftUnitDetailWindowPosition().X + LeftUnitDetailWindow.Width + WindowEdgeBuffer,
                LeftUnitDetailWindowPosition().Y
            );
        }


        private Vector2 RightUnitPortraitWindowPosition()
        {
            //Bottom-right, above intiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - RightUnitPortraitWindow.Height - BlueTeamWindow.Height
            );
        }

        private Vector2 RightUnitDetailWindowPosition()
        {
            //Bottom-right, left of portrait, above intiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitDetailWindow.Width - RightUnitPortraitWindow.Width -
                WindowEdgeBuffer,
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


        private Vector2 RightUnitInventoryWindowPosition()
        {
            //Bottom-left, right of stats
            return new Vector2(
                RightUnitDetailWindowPosition().X - RightUnitInventoryWindow.Width - WindowEdgeBuffer,
                RightUnitDetailWindowPosition().Y
            );
        }

        private Vector2 InitiativeWindowPosition()
        {
            //Bottom-center
            return new Vector2(
                GameDriver.ScreenSize.X / 2 -
                (float) InitiativeWindow.Width / 2,
                GameDriver.ScreenSize.Y - BlueTeamWindow.Height
            );
        }

        private Vector2 EntityWindowPosition()
        {
            //Top-right
            return new Vector2(
                GameDriver.ScreenSize.X - EntityWindow.Width - WindowEdgeBuffer,
                WindowEdgeBuffer
            );
        }

        private Vector2 ObjectiveWindowPosition()
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

        private Vector2 MenuDescriptionWindowPosition(Vector2 menuPosition)
        {
            return menuPosition - new Vector2(0, MenuDescriptionWindow.Height);
        }

        #endregion Window Positions

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (EntityWindow != null)
            {
                EntityWindow.Draw(spriteBatch, EntityWindowPosition());
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

                    if (LeftUnitInventoryWindow != null)
                    {
                        LeftUnitInventoryWindow.Draw(spriteBatch, LeftUnitInventoryWindowPosition());
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

                    if (RightUnitInventoryWindow != null)
                    {
                        RightUnitInventoryWindow.Draw(spriteBatch, RightUnitInventoryWindowPosition());
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

                if (ActionMenu.IsVisible)
                {
                    if (ActionMenuDescriptionWindow != null)
                    {
                        ActionMenuDescriptionWindow.Draw(spriteBatch, ActionMenuDescriptionPosition());
                    }

                    if (MenuDescriptionWindow != null)
                    {
                        MenuDescriptionWindow.Draw(spriteBatch, MenuDescriptionWindowPosition(ActionMenuPosition()));
                    }
                }
            }

            if (InventoryMenu != null)
            {
                InventoryMenu.Draw(spriteBatch, InventoryMenuPosition());
                if (InventoryMenu.IsVisible)
                {
                    if (InventoryMenuDescriptionWindow != null)
                    {
                        InventoryMenuDescriptionWindow.Draw(spriteBatch, InventoryMenuDescriptionPosition());
                    }

                    if (MenuDescriptionWindow != null)
                    {
                        MenuDescriptionWindow.Draw(spriteBatch, MenuDescriptionWindowPosition(InventoryMenuPosition()));
                    }
                }
            }

            if (ObjectiveWindow != null)
            {
                ObjectiveWindow.Draw(spriteBatch, ObjectiveWindowPosition());
            }
        }
    }
}