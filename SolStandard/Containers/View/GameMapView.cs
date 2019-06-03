using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity;
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
        private enum MenuType
        {
            ActionMenu,
            InventoryMenu
        }

        private const int WindowEdgeBuffer = 5;
        public static readonly Color TeamListWindowBackgroundColor = new Color(0, 0, 0, 50);
        public static readonly Color BlankTerrainWindowColor = new Color(30, 30, 30, 180);
        public static readonly Color ItemTerrainWindowColor = new Color(120, 120, 60, 180);
        public static readonly Color EntityTerrainWindowColor = new Color(50, 100, 50, 180);
        public static readonly Color UserPromptWindowColor = new Color(40, 30, 40, 200);

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

        public Window ItemDetailWindow { get; private set; }
        private Window UserPromptWindow { get; set; }

        private VerticalMenu ActionMenu { get; set; }
        private Window ActionMenuDescriptionWindow { get; set; }

        private VerticalMenu InventoryMenu { get; set; }
        private Window InventoryMenuDescriptionWindow { get; set; }

        private Window MenuDescriptionWindow { get; set; }

        public TwoDimensionalMenu AdHocDraftMenu { get; private set; }

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

        public void CloseItemDetailWindow()
        {
            ItemDetailWindow = null;
        }

        public void CloseCombatMenu()
        {
            ActionMenu.IsVisible = false;
            InventoryMenu.IsVisible = false;
        }

        public void CloseAdHocDraftMenu()
        {
            AdHocDraftMenu = null;
        }

        #endregion Close Windows

        #region Generation

        public void GenerateDraftMenu(Team team)
        {
            AdHocDraftMenu = new TwoDimensionalMenu(
                DraftView.GetAdHocUnitOptionsForTeam(team, new Dictionary<Role, bool>()),
                DraftView.DraftCursor,
                TeamUtility.DetermineTeamColor(team),
                TwoDimensionalMenu.CursorType.Frame
            );
        }

        public void GenerateItemDetailWindow(List<IItem> items)
        {
            ItemDetailWindow = GenerateItemsWindow(items, ItemTerrainWindowColor);
        }

        public static Window GenerateItemsWindow(IReadOnlyList<IItem> items, Color windowColor)
        {
            IRenderable[,] actionElements = new IRenderable[items.Count, 3];

            const int iconIndex = 0;
            const int nameIndex = 1;
            const int descriptionIndex = 2;

            int largestNameWidth = 0;
            int largestDescriptionWidth = 0;

            for (int i = 0; i < items.Count; i++)
            {
                actionElements[i, iconIndex] = items[i].Icon.Clone();

                actionElements[i, nameIndex] =
                    new Window(new RenderText(AssetManager.WindowFont, items[i].Name), Color.Transparent);

                actionElements[i, descriptionIndex] =
                    new Window(items[i].UseAction().Description, windowColor);

                //Remember the largest width for aligning later
                if (actionElements[i, nameIndex].Width > largestNameWidth)
                {
                    largestNameWidth = actionElements[i, nameIndex].Width;
                }

                if (actionElements[i, descriptionIndex].Width > largestDescriptionWidth)
                {
                    largestDescriptionWidth = actionElements[i, descriptionIndex].Width;
                }
            }


            for (int i = 0; i < items.Count; i++)
            {
                //Fill space so that all the elements have the same width like a grid
                ((Window) actionElements[i, nameIndex]).Width = largestNameWidth;
                ((Window) actionElements[i, descriptionIndex]).Width = largestDescriptionWidth;
            }

            Window itemTable = new Window(new WindowContentGrid(actionElements, 5), windowColor);


            return new Window(new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.HeaderFont, "ITEMS"),
                    },
                    {
                        itemTable
                    }
                },
                3,
                HorizontalAlignment.Centered
            ), windowColor);
        }

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
                UnitContextualActionMenuContext.GetActionDescriptionAtIndex(ActionMenu),
                windowColor
            );
        }

        private void GenerateInventoryMenuDescription(Color windowColor)
        {
            InventoryMenuDescriptionWindow = new Window(
                UnitContextualActionMenuContext.GetActionDescriptionAtIndex(InventoryMenu),
                windowColor
            );
        }

        public void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            UserPromptWindow = new Window(
                promptTextContent,
                UserPromptWindowColor,
                sizeOverride
            );
        }

        public void SetEntityWindow(MapSlice hoverSlice)
        {
            EntityWindow = GenerateEntityWindow(hoverSlice);
        }

        public static Window GenerateEntityWindow(MapSlice hoverSlice)
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
                                    EntityTerrainWindowColor
                                ) as IRenderable
                                : new RenderBlank()
                        },
                        {
                            (hoverSlice.ItemEntity != null)
                                ? new Window(
                                    hoverSlice.ItemEntity.TerrainInfo,
                                    ItemTerrainWindowColor
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
                                BlankTerrainWindowColor
                            )
                        }
                    },
                    1);
            }

            return new Window(terrainContentGrid, new Color(50, 50, 50, 150));
        }

        public void GenerateObjectiveWindow()
        {
            ObjectiveWindow = GameContext.Scenario.ScenarioInfo();
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
            const int unitsPerRow = 10;
            const int initiativeHealthBarHeight = 10;


            List<GameUnit> unitList = GameContext.Units.FindAll(unit => unit.Team == team);

            int rows = Convert.ToInt32(Math.Ceiling((float) unitList.Count / unitsPerRow));

            IRenderable[,] unitListGrid = new IRenderable[rows, unitsPerRow];

            int unitIndex = 0;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex < unitList.Count)
                    {
                        unitListGrid[row, column] =
                            SingleUnitContent(unitList[unitIndex], initiativeHealthBarHeight, null);
                    }
                    else
                    {
                        unitListGrid[row, column] = new RenderBlank();
                    }

                    unitIndex++;
                }
            }

            WindowContentGrid unitListContentGrid =
                new WindowContentGrid(unitListGrid, 0, HorizontalAlignment.Centered);

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

        public static IRenderable SingleUnitContent(GameUnit unit, int initiativeHealthBarHeight,
            Color? windowColorOverride)
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
                windowColorOverride ?? TeamUtility.DetermineTeamColor(unit.Team)
            );
            return singleUnitContent;
        }

        public void UpdateLeftPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            if (hoverMapUnit == null)
            {
                LeftUnitPortraitWindow = null;
                LeftUnitDetailWindow = null;
                LeftUnitStatusWindow = null;
                LeftUnitInventoryWindow = null;
            }
            else
            {
                Color windowColor = TeamUtility.DetermineTeamColor(hoverMapUnit.Team);
                LeftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                LeftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                LeftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                LeftUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);
            }
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            if (hoverMapUnit == null)
            {
                RightUnitPortraitWindow = null;
                RightUnitDetailWindow = null;
                RightUnitStatusWindow = null;
                RightUnitInventoryWindow = null;
            }
            else
            {
                Color windowColor = TeamUtility.DetermineTeamColor(hoverMapUnit.Team);
                RightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                RightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                RightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                RightUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);
            }
        }


        public static Window GenerateUnitPortraitWindow(IRenderable unitPortraitPane, Color windowColor)
        {
            return new Window(unitPortraitPane, windowColor);
        }

        public static Window GenerateUnitDetailWindow(IRenderable selectedUnitInfo, Color windowColor)
        {
            return new Window(selectedUnitInfo, windowColor);
        }

        private static Window GenerateUnitStatusWindow(IReadOnlyList<StatusEffect> statusEffects, Color windowColor)
        {
            if (statusEffects.Count < 1) return null;


            IRenderable[,] selectedUnitStatuses = new IRenderable[statusEffects.Count, 1];

            for (int i = 0; i < statusEffects.Count; i++)
            {
                StatusEffect effect = statusEffects[i];

                selectedUnitStatuses[i, 0] = new Window(
                    new WindowContentGrid(
                        new[,]
                        {
                            {
                                StatusIconProvider.GetStatusIcon(StatusIcon.Time, new Vector2(GameDriver.CellSize)),
                                new RenderText(AssetManager.WindowFont, "[" + effect.TurnDuration + "] "),
                                effect.StatusIcon,
                                new RenderText(AssetManager.WindowFont, effect.Name,
                                    (effect.CanCleanse) ? Color.Red : Color.LightGreen)
                            }
                        },
                        0
                    ),
                    windowColor
                );
            }

            return new Window(new WindowContentGrid(selectedUnitStatuses, 1), windowColor);
        }

        private static Window GenerateUnitInventoryWindow(IRenderable inventoryPane, Color windowColor)
        {
            return (inventoryPane == null) ? null : new Window(inventoryPane, windowColor);
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
                GameDriver.ScreenSize.Y - LeftUnitPortraitWindow.Height - Math.Max(BlueTeamWindow.Height, RedTeamWindow.Height)
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
                LeftUnitDetailWindowPosition().Y - LeftUnitStatusWindow.Height - WindowEdgeBuffer
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
            //Bottom-right, above initiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - RightUnitPortraitWindow.Height - Math.Max(BlueTeamWindow.Height, RedTeamWindow.Height)
            );
        }

        private Vector2 RightUnitDetailWindowPosition()
        {
            //Bottom-right, left of portrait, above initiative window
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
                RightUnitDetailWindowPosition().Y - RightUnitStatusWindow.Height - WindowEdgeBuffer
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
                GameDriver.ScreenSize.Y - Math.Max(BlueTeamWindow.Height, RedTeamWindow.Height)
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

        private Vector2 ItemDetailWindowPosition()
        {
            //Middle of the screen
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) ItemDetailWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) ItemDetailWindow.Height / 2
            );
        }

        private Vector2 AdHocDraftMenuPosition()
        {
            //Middle of the screen
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) AdHocDraftMenu.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) AdHocDraftMenu.Height / 2
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

                if (ItemDetailWindow != null)
                {
                    ItemDetailWindow.Draw(spriteBatch, ItemDetailWindowPosition());
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

            if (AdHocDraftMenu != null)
            {
                AdHocDraftMenu.Draw(spriteBatch, AdHocDraftMenuPosition());
            }
        }
    }
}