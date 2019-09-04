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
using SolStandard.HUD.Menu.Options.StealMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

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
            InventoryMenu,
            StealItemMenu,
            DraftMenu
        }

        private const int WindowSlideSpeed = 40;
        private const int WindowSlideDistance = 300;

        private const int WindowEdgeBuffer = 5;
        public static readonly Color TeamListWindowBackgroundColor = new Color(0, 0, 0, 50);
        public static readonly Color BlankTerrainWindowColor = new Color(30, 30, 30, 180);
        public static readonly Color ItemTerrainWindowColor = new Color(120, 120, 60, 180);
        public static readonly Color EntityTerrainWindowColor = new Color(50, 100, 50, 180);
        public static readonly Color UserPromptWindowColor = new Color(40, 30, 40, 200);

        private int LeftHoverUnit { get; set; }
        private AnimatedWindow LeftUnitPortraitWindow { get; set; }
        private AnimatedWindow LeftUnitDetailWindow { get; set; }
        private AnimatedWindow LeftUnitStatusWindow { get; set; }
        private AnimatedWindow LeftUnitInventoryWindow { get; set; }

        private int RightHoverUnit { get; set; }
        private AnimatedWindow RightUnitPortraitWindow { get; set; }
        private AnimatedWindow RightUnitDetailWindow { get; set; }
        private AnimatedWindow RightUnitStatusWindow { get; set; }
        private AnimatedWindow RightUnitInventoryWindow { get; set; }

        private int EntityWindowHash { get; set; }
        private AnimatedWindow EntityWindow { get; set; }

        private Window InitiativeWindow { get; set; }
        private Window BlueTeamWindow { get; set; }
        private Window RedTeamWindow { get; set; }
        private Window ObjectiveWindow { get; set; }

        public Window ItemDetailWindow { get; private set; }
        private Window UserPromptWindow { get; set; }

        private VerticalMenu ActionMenu { get; set; }
        private Window ActionMenuDescriptionWindow { get; set; }

        private TwoDimensionalMenu InventoryMenu { get; set; }
        private Window InventoryMenuDescriptionWindow { get; set; }

        private Window MenuDescriptionWindow { get; set; }

        private TwoDimensionalMenu AdHocDraftMenu { get; set; }
        private TwoDimensionalMenu StealItemMenu { get; set; }
        private readonly IRenderable cursorSprite;

        private MenuType visibleMenu;
        private bool visible;

        public GameMapView()
        {
            visible = true;
            cursorSprite = new SpriteAtlas(AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width, AssetManager.MenuCursorTexture.Height));
        }

        private MenuType VisibleMenu
        {
            get => visibleMenu;
            set
            {
                visibleMenu = value;
                switch (value)
                {
                    case MenuType.ActionMenu:
                        ActionMenu.IsVisible = true;
                        InventoryMenu.IsVisible = false;
                        if (StealItemMenu != null) StealItemMenu.IsVisible = false;
                        if (AdHocDraftMenu != null) AdHocDraftMenu.IsVisible = false;
                        break;
                    case MenuType.InventoryMenu:
                        ActionMenu.IsVisible = false;
                        InventoryMenu.IsVisible = true;
                        if (StealItemMenu != null) StealItemMenu.IsVisible = false;
                        if (AdHocDraftMenu != null) AdHocDraftMenu.IsVisible = false;
                        break;
                    case MenuType.StealItemMenu:
                        ActionMenu.IsVisible = false;
                        InventoryMenu.IsVisible = false;
                        StealItemMenu.IsVisible = true;
                        if (AdHocDraftMenu != null) AdHocDraftMenu.IsVisible = false;
                        break;
                    case MenuType.DraftMenu:
                        ActionMenu.IsVisible = false;
                        InventoryMenu.IsVisible = false;
                        if (StealItemMenu != null) StealItemMenu.IsVisible = false;
                        AdHocDraftMenu.IsVisible = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public IMenu CurrentMenu
        {
            get
            {
                switch (VisibleMenu)
                {
                    case MenuType.ActionMenu:
                        return ActionMenu;
                    case MenuType.InventoryMenu:
                        return InventoryMenu;
                    case MenuType.StealItemMenu:
                        return StealItemMenu;
                    case MenuType.DraftMenu:
                        return AdHocDraftMenu;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static IWindowAnimation RightSideWindowAnimation =>
            new WindowSlide(WindowSlide.SlideDirection.Left, WindowSlideDistance, WindowSlideSpeed);

        private static IWindowAnimation LeftSideWindowAnimation =>
            new WindowSlide(WindowSlide.SlideDirection.Right, WindowSlideDistance, WindowSlideSpeed);

        public void ToggleCombatMenu()
        {
            switch (VisibleMenu)
            {
                case MenuType.ActionMenu:
                    VisibleMenu = MenuType.InventoryMenu;
                    break;
                case MenuType.InventoryMenu:
                    VisibleMenu = MenuType.ActionMenu;
                    break;
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

        public void CloseStealItemMenu()
        {
            StealItemMenu = null;
        }

        #endregion Close Windows

        #region Generation

        public void GenerateStealItemMenu(GameUnit targetToStealFrom)
        {
            StealItemMenu = new TwoDimensionalMenu(
                GenerateStealOptions(targetToStealFrom),
                cursorSprite,
                ItemTerrainWindowColor,
                TwoDimensionalMenu.CursorType.Pointer
            );
            VisibleMenu = MenuType.StealItemMenu;
        }

        private static MenuOption[,] GenerateStealOptions(GameUnit targetToStealFrom)
        {
            List<IItem> unitInventory = targetToStealFrom.Inventory;
            MenuOption[,] menu = new MenuOption[unitInventory.Count, 1];

            for (int i = 0; i < unitInventory.Count; i++)
            {
                menu[i, 0] = new StealItemOption(targetToStealFrom, unitInventory[i], ItemTerrainWindowColor);
            }

            return menu;
        }

        public void GenerateDraftMenu(Team team)
        {
            AdHocDraftMenu = new TwoDimensionalMenu(
                DraftView.GetAdHocUnitOptionsForTeam(team, new Dictionary<Role, bool>()),
                DraftView.DraftCursor,
                TeamUtility.DetermineTeamColor(team),
                TwoDimensionalMenu.CursorType.Frame
            );
            VisibleMenu = MenuType.DraftMenu;
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
                        new RenderText(AssetManager.HeaderFont, "ITEMS")
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

        private void GenerateActionMenu(Color windowColor)
        {
            MenuOption[] options = UnitContextualActionMenuContext.GenerateActionMenuOptions(windowColor);
            ActionMenu = new VerticalMenu(options, cursorSprite, windowColor);
        }

        private void GenerateInventoryMenu(Color windowColor)
        {
            MenuOption[,] options = UnitContextualActionMenuContext.GenerateInventoryMenuOptions(windowColor);
            InventoryMenu =
                new TwoDimensionalMenu(options, cursorSprite, windowColor, TwoDimensionalMenu.CursorType.Pointer);
        }

        private void GenerateMenuDescriptionWindow(MenuType menuType, Color windowColor)
        {
            string menuName;
            RenderText windowText;

            switch (menuType)
            {
                case MenuType.ActionMenu:
                    menuName = "Unit Actions";
                    windowText = new RenderText(AssetManager.HeaderFont, menuName);
                    break;
                case MenuType.InventoryMenu:
                    menuName = "Inventory";
                    windowText = new RenderText(AssetManager.HeaderFont, menuName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(menuType), menuType, null);
            }


            MenuDescriptionWindow = new Window(
                new WindowContentGrid(
                    new[,]
                    {
                        {
                            InputIconProvider.GetInputIcon(Input.PreviewUnit, new Vector2(windowText.Height)),
                            windowText,
                            InputIconProvider.GetInputIcon(Input.PreviewItem, new Vector2(windowText.Height))
                        }
                    },
                    3,
                    HorizontalAlignment.Centered
                ),
                windowColor,
                HorizontalAlignment.Centered
            );
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
            if (EntityWindowHash == hoverSlice.GetHashCode()) return;
            EntityWindowHash = hoverSlice.GetHashCode();
            EntityWindow = new AnimatedWindow(GenerateEntityWindow(hoverSlice), RightSideWindowAnimation);
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
                            new RenderText(AssetManager.WindowFont,
                                $"[ X: {GameContext.MapCursor.MapCoordinates.X}, Y: {GameContext.MapCursor.MapCoordinates.Y} ]"),
                            new RenderBlank()
                        },
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
                    1,
                    HorizontalAlignment.Centered
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
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
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
                    unit.IsCommander
                        ? unit.GetInitiativeCommandPointBar(new Vector2(unit.SmallPortrait.Width,
                            (float) initiativeHealthBarHeight / 2))
                        : new RenderBlank() as IRenderable
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
                LeftHoverUnit = 0;
                LeftUnitPortraitWindow = null;
                LeftUnitDetailWindow = null;
                LeftUnitStatusWindow = null;
                LeftUnitInventoryWindow = null;
            }
            else
            {
                if (!hoverMapUnit.IsDifferentFrom(LeftHoverUnit)) return;
                LeftHoverUnit = hoverMapUnit.GetHashCode();
                Color windowColor = TeamUtility.DetermineTeamColor(hoverMapUnit.Team);
                Window leftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                Window leftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                Window leftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                Window leftUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);

                LeftUnitPortraitWindow = new AnimatedWindow(leftUnitPortraitWindow, LeftSideWindowAnimation);
                LeftUnitDetailWindow = new AnimatedWindow(leftUnitDetailWindow, LeftSideWindowAnimation);

                LeftUnitStatusWindow = leftUnitStatusWindow != null
                    ? new AnimatedWindow(leftUnitStatusWindow, LeftSideWindowAnimation)
                    : null;
                LeftUnitInventoryWindow = leftUnitInventoryWindow != null
                    ? new AnimatedWindow(leftUnitInventoryWindow, LeftSideWindowAnimation)
                    : null;
            }
        }

        public void UpdateRightPortraitAndDetailWindows(GameUnit hoverMapUnit)
        {
            if (hoverMapUnit == null)
            {
                RightHoverUnit = 0;
                RightUnitPortraitWindow = null;
                RightUnitDetailWindow = null;
                RightUnitStatusWindow = null;
                RightUnitInventoryWindow = null;
            }
            else
            {
                if (!hoverMapUnit.IsDifferentFrom(RightHoverUnit)) return;
                RightHoverUnit = hoverMapUnit.GetHashCode();
                Color windowColor = TeamUtility.DetermineTeamColor(hoverMapUnit.Team);
                Window rightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                Window rightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                Window rightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                Window rightUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);


                RightUnitPortraitWindow = new AnimatedWindow(rightUnitPortraitWindow, RightSideWindowAnimation);
                RightUnitDetailWindow = new AnimatedWindow(rightUnitDetailWindow, RightSideWindowAnimation);
                RightUnitStatusWindow = rightUnitStatusWindow != null
                    ? new AnimatedWindow(rightUnitStatusWindow, RightSideWindowAnimation)
                    : null;
                RightUnitInventoryWindow = rightUnitInventoryWindow != null
                    ? new AnimatedWindow(rightUnitInventoryWindow, RightSideWindowAnimation)
                    : null;
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
                                StatusIconProvider.GetStatusIcon(StatusIcon.Time, GameDriver.CellSizeVector),
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
                GameDriver.ScreenSize.X / 3 - (float) ActionMenu.Width / 2,
                (GameDriver.ScreenSize.Y / 2) - ((float) ActionMenu.Height / 2)
            );
        }

        private Vector2 InventoryMenuPosition()
        {
            //Center of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 3 - (float) InventoryMenu.Width / 2,
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
                GameDriver.ScreenSize.Y - LeftUnitPortraitWindow.Height -
                Math.Max(BlueTeamWindow.Height, RedTeamWindow.Height)
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
            //Bottom-right, above initiative window
            return new Vector2(
                GameDriver.ScreenSize.X - RightUnitPortraitWindow.Width - WindowEdgeBuffer,
                GameDriver.ScreenSize.Y - RightUnitPortraitWindow.Height -
                Math.Max(BlueTeamWindow.Height, RedTeamWindow.Height)
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
            return CenterItemOnScreen(UserPromptWindow);
        }

        private Vector2 ItemDetailWindowPosition()
        {
            return CenterItemOnScreen(ItemDetailWindow);
        }

        private Vector2 AdHocDraftMenuPosition()
        {
            return CenterItemOnScreen(AdHocDraftMenu);
        }

        private Vector2 SteamItemMenuPosition()
        {
            return CenterItemOnScreen(StealItemMenu);
        }

        private static Vector2 CenterItemOnScreen(IRenderable item)
        {
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) item.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) item.Height / 2
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
            EntityWindow?.Draw(spriteBatch, EntityWindowPosition());
            if (InitiativeWindow != null)
            {
                InitiativeWindow.Draw(spriteBatch, InitiativeWindowPosition());

                if (LeftUnitPortraitWindow != null)
                {
                    LeftUnitPortraitWindow.Draw(spriteBatch, LeftUnitPortraitWindowPosition());
                    LeftUnitDetailWindow?.Draw(spriteBatch, LeftUnitDetailWindowPosition());
                    LeftUnitStatusWindow?.Draw(spriteBatch, LeftUnitStatusWindowPosition());
                    LeftUnitInventoryWindow?.Draw(spriteBatch, LeftUnitInventoryWindowPosition());
                }

                if (RightUnitPortraitWindow != null)
                {
                    RightUnitPortraitWindow.Draw(spriteBatch, RightUnitPortraitWindowPosition());
                    RightUnitDetailWindow?.Draw(spriteBatch, RightUnitDetailWindowPosition());
                    RightUnitStatusWindow?.Draw(spriteBatch, RightUnitStatusWindowPosition());
                    RightUnitInventoryWindow?.Draw(spriteBatch, RightUnitInventoryWindowPosition());
                }

                ItemDetailWindow?.Draw(spriteBatch, ItemDetailWindowPosition());
                UserPromptWindow?.Draw(spriteBatch, UserPromptWindowPosition());
            }

            if (ActionMenu != null)
            {
                ActionMenu.Draw(spriteBatch, ActionMenuPosition());

                if (ActionMenu.IsVisible)
                {
                    ActionMenuDescriptionWindow?.Draw(spriteBatch, ActionMenuDescriptionPosition());
                    MenuDescriptionWindow?.Draw(spriteBatch, MenuDescriptionWindowPosition(ActionMenuPosition()));
                }
            }

            if (InventoryMenu != null)
            {
                InventoryMenu.Draw(spriteBatch, InventoryMenuPosition());
                if (InventoryMenu.IsVisible)
                {
                    InventoryMenuDescriptionWindow?.Draw(spriteBatch, InventoryMenuDescriptionPosition());
                    MenuDescriptionWindow?.Draw(spriteBatch, MenuDescriptionWindowPosition(InventoryMenuPosition()));
                }
            }

            ObjectiveWindow?.Draw(spriteBatch, ObjectiveWindowPosition());
            AdHocDraftMenu?.Draw(spriteBatch, AdHocDraftMenuPosition());
            StealItemMenu?.Draw(spriteBatch, SteamItemMenuPosition());
        }
    }
}