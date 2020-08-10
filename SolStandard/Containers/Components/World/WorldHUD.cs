using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Draft;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.World.SubContext.ActionMenu;
using SolStandard.Containers.Components.World.SubContext.Movement;
using SolStandard.Containers.Scenario;
using SolStandard.Entity;
using SolStandard.Entity.General;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.Entity.Unit.Actions.Champion;
using SolStandard.Entity.Unit.Actions.Duelist;
using SolStandard.Entity.Unit.Statuses;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.ActionMenu;
using SolStandard.HUD.Menu.Options.StealMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;

namespace SolStandard.Containers.Components.World
{
    /*
     * GameMapContext is where the HUD elements for the SelectMapEntity Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class WorldHUD : IUserInterface
    {
        private enum MenuType
        {
            ActionMenu,
            TakeItemMenu,
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
        private AnimatedRenderable LeftUnitPortraitWindow { get; set; }
        private AnimatedRenderable LeftUnitDetailWindow { get; set; }
        private AnimatedRenderable LeftUnitStatusWindow { get; set; }
        private AnimatedRenderable LeftUnitInventoryWindow { get; set; }

        private int RightHoverUnit { get; set; }
        private AnimatedRenderable RightUnitPortraitWindow { get; set; }
        private AnimatedRenderable RightUnitDetailWindow { get; set; }
        private AnimatedRenderable RightUnitStatusWindow { get; set; }
        private AnimatedRenderable RightUnitInventoryWindow { get; set; }

        private int EntityWindowHash { get; set; }
        private AnimatedRenderable EntityWindow { get; set; }

        private Window InitiativeWindow { get; set; }
        private Window BlueTeamWindow { get; set; }
        private Window RedTeamWindow { get; set; }
        private Window ObjectiveWindow { get; set; }
        private static Window TeamInfoWindow => GenerateTeamInfoWindow();

        public Window ItemDetailWindow { get; private set; }
        private Window UserPromptWindow { get; set; }

        private Window MenuHeaderWindow { get; set; }
        public MenuContext ActionMenuContext { get; private set; }
        private Window ActionMenuDescriptionWindow { get; set; }

        private TwoDimensionalMenu AdHocDraftMenu { get; set; }
        private TwoDimensionalMenu TakeItemMenu { get; set; }
        private readonly IRenderable cursorSprite;

        private IRenderable CenterScreenContent { get; set; }

        private MenuType visibleMenu;

        public WorldHUD()
        {
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
                        ActionMenuContext.CurrentMenu.IsVisible = true;
                        if (TakeItemMenu != null) TakeItemMenu.IsVisible = false;
                        if (AdHocDraftMenu != null) AdHocDraftMenu.IsVisible = false;
                        break;
                    case MenuType.TakeItemMenu:
                        ActionMenuContext.CurrentMenu.IsVisible = false;
                        TakeItemMenu.IsVisible = true;
                        if (AdHocDraftMenu != null) AdHocDraftMenu.IsVisible = false;
                        break;
                    case MenuType.DraftMenu:
                        ActionMenuContext.CurrentMenu.IsVisible = false;
                        if (TakeItemMenu != null) TakeItemMenu.IsVisible = false;
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
                return VisibleMenu switch
                {
                    MenuType.ActionMenu => ActionMenuContext.CurrentMenu,
                    MenuType.TakeItemMenu => TakeItemMenu,
                    MenuType.DraftMenu => AdHocDraftMenu,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private static IRenderableAnimation RightSideWindowAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Left, WindowSlideDistance, WindowSlideSpeed);

        private static IRenderableAnimation LeftSideWindowAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Right, WindowSlideDistance, WindowSlideSpeed);

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
            ActionMenuContext.ClearMenuStack();
        }

        public void CloseAdHocDraftMenu()
        {
            AdHocDraftMenu = null;
        }

        public void CloseStealItemMenu()
        {
            TakeItemMenu = null;
        }

        public void StopRenderingCenterScreenContent()
        {
            CenterScreenContent = null;
        }

        #endregion Close Windows

        #region Generation

        public void RenderCenterScreen(IRenderable content)
        {
            CenterScreenContent = content;
        }

        public void GenerateTakeItemMenu(GameUnit targetToTakeFrom, bool freeAction)
        {
            TakeItemMenu = new TwoDimensionalMenu(
                GenerateTakeOptions(targetToTakeFrom, freeAction),
                cursorSprite,
                ItemTerrainWindowColor,
                TwoDimensionalMenu.CursorType.Pointer
            );
            VisibleMenu = MenuType.TakeItemMenu;
        }

        private static MenuOption[,] GenerateTakeOptions(GameUnit targetToTakeFrom, bool freeAction)
        {
            List<IItem> unitInventory = targetToTakeFrom.Inventory;
            var menu = new MenuOption[unitInventory.Count, 1];

            for (int i = 0; i < unitInventory.Count; i++)
            {
                menu[i, 0] = new TakeItemOption(targetToTakeFrom, unitInventory[i], ItemTerrainWindowColor, freeAction);
            }

            return menu;
        }

        public void GenerateDraftMenu(Team team)
        {
            AdHocDraftMenu = new TwoDimensionalMenu(
                DraftHUD.GetAdHocUnitOptionsForTeam(team, new Dictionary<Role, bool>()),
                DraftHUD.DraftCursor,
                TeamUtility.DetermineTeamWindowColor(team),
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
            var actionElements = new IRenderable[items.Count, 3];

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

            var itemTable = new Window(new WindowContentGrid(actionElements, 5), windowColor);


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

        private static Window GenerateTeamInfoWindow()
        {
            ISpriteFont font = AssetManager.WindowFont;

            var blueGoldWindow = new Window(
                new RenderText(font, $"Blue: {GlobalContext.InitiativePhase.GetGoldForTeam(Team.Blue)}G"),
                TeamUtility.DetermineTeamWindowColor(Team.Blue));

            var redGoldWindow = new Window(
                new RenderText(font, $"Red: {GlobalContext.InitiativePhase.GetGoldForTeam(Team.Red)}G"),
                TeamUtility.DetermineTeamWindowColor(Team.Red));


            bool blueIsFirst = GlobalContext.InitiativePhase.TeamWithFewerRemainingUnits == Team.Blue;
            IRenderable firstIcon = MiscIconProvider.GetMiscIcon(MiscIcon.First, GameDriver.CellSizeVector);
            IRenderable secondIcon = MiscIconProvider.GetMiscIcon(MiscIcon.Second, GameDriver.CellSizeVector);

            var teamGoldWindowContentGrid = new WindowContentGrid(
                new[,]
                {
                    {
                        blueIsFirst ? firstIcon : secondIcon,
                        blueGoldWindow,
                        ObjectiveIconProvider.GetObjectiveIcon(
                            VictoryConditions.Taxes,
                            GameDriver.CellSizeVector
                        ),
                        redGoldWindow,
                        blueIsFirst ? secondIcon : firstIcon
                    }
                },
                2,
                HorizontalAlignment.Centered
            );

            return new Window(teamGoldWindowContentGrid, BlankTerrainWindowColor);
        }

        public void GenerateActionMenus()
        {
            Color windowColor = TeamUtility.DetermineTeamWindowColor(GlobalContext.ActiveTeam);

            IMenu contextMenu = BuildContextMenu(windowColor);

            List<ActionOption> skillOptions = ContextMenuUtils.ActiveUnitSkillOptions(windowColor);
            ActionOption basicAttackOption = skillOptions.FirstOrDefault(option => option.Action is BasicAttack);
            ActionOption waitOption =
                skillOptions.FirstOrDefault(option => option.Action is Wait || option.Action is Focus);
            ActionOption roleOption =
                skillOptions.FirstOrDefault(option => option.Action is Shove || option.Action is Sprint);
            ActionOption guardOption = skillOptions.FirstOrDefault(option => option.Action is Guard);

            skillOptions.Remove(basicAttackOption);
            skillOptions.Remove(waitOption);
            skillOptions.Remove(roleOption);
            skillOptions.Remove(guardOption);

            IMenu skillMenu = BuildSkillMenu(skillOptions, windowColor);

            IMenu inventoryMenu = BuildInventoryMenu(windowColor);

            IMenu actionMenu = BuildActionMenu(contextMenu, windowColor, basicAttackOption, skillMenu, inventoryMenu,
                roleOption, guardOption, waitOption);
            ActionMenuContext = new MenuContext(actionMenu);

            VisibleMenu = MenuType.ActionMenu;
            GenerateMenuDescriptionWindow(VisibleMenu, windowColor);
        }

        private IMenu BuildActionMenu(IMenu contextMenu, Color windowColor, MenuOption basicAttackOption,
            IMenu skillMenu, IMenu inventoryMenu, MenuOption roleOption, MenuOption guardOption, MenuOption waitOption)
        {
            var topLevelOptions = new List<MenuOption>();

            if (contextMenu != null)
            {
                IRenderable contextMenuIcon = MiscIconProvider.GetMiscIcon(MiscIcon.Context, GameDriver.CellSizeVector);
                topLevelOptions.Add(new SubmenuOption(contextMenu, contextMenuIcon, "Context",
                    "View extra actions that can be performed based on the environment.", windowColor));
            }

            if (basicAttackOption != null) topLevelOptions.Add(basicAttackOption);

            if (skillMenu != null)
            {
                IRenderable skillMenuIcon = MiscIconProvider.GetMiscIcon(MiscIcon.SkillBook, GameDriver.CellSizeVector);
                topLevelOptions.Add(new SubmenuOption(skillMenu, skillMenuIcon, "Skills",
                    "View unique actions for this unit.", windowColor));
            }

            if (inventoryMenu != null)
            {
                IRenderable spoilsMenuIcon = MiscIconProvider.GetMiscIcon(MiscIcon.Spoils, GameDriver.CellSizeVector);
                topLevelOptions.Add(new SubmenuOption(inventoryMenu, spoilsMenuIcon, "Inventory",
                    "Use and manage items in this unit's inventory.", windowColor));
            }

            if (roleOption != null) topLevelOptions.Add(roleOption);
            if (guardOption != null) topLevelOptions.Add(guardOption);
            if (waitOption != null) topLevelOptions.Add(waitOption);


            IMenu actionMenu = new VerticalMenu(topLevelOptions.ToArray(), cursorSprite, windowColor);
            return actionMenu;
        }

        private IMenu BuildInventoryMenu(Color windowColor)
        {
            MenuOption[,] inventoryOptions = ContextMenuUtils.GenerateInventoryMenuOptions(windowColor);
            return (inventoryOptions.Length > 0)
                ? new TwoDimensionalMenu(inventoryOptions, cursorSprite, windowColor,
                    TwoDimensionalMenu.CursorType.Pointer)
                : null;
        }

        private IMenu BuildSkillMenu(List<ActionOption> skillOptions, Color windowColor)
        {
            IMenu skillMenu = null;
            // ReSharper disable once CoVariantArrayConversion
            if (skillOptions.Count > 0) skillMenu = new VerticalMenu(skillOptions.ToArray(), cursorSprite, windowColor);
            return skillMenu;
        }

        private IMenu BuildContextMenu(Color windowColor)
        {
            // ReSharper disable once CoVariantArrayConversion
            MenuOption[] contextOptions =
                ContextMenuUtils.ActiveUnitContextOptions(windowColor).ToArray();
            IMenu contextMenu = null;
            if (contextOptions.Length > 0) contextMenu = new VerticalMenu(contextOptions, cursorSprite, windowColor);
            return contextMenu;
        }

        private void GenerateMenuDescriptionWindow(MenuType menuType, Color windowColor)
        {
            RenderText windowText;

            switch (menuType)
            {
                case MenuType.ActionMenu:
                    const string menuName = "Unit Actions";
                    windowText = new RenderText(AssetManager.HeaderFont, menuName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(menuType), menuType, null);
            }


            MenuHeaderWindow = new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
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

        public void GenerateCurrentMenuDescription()
        {
            Color windowColor = TeamUtility.DetermineTeamWindowColor(GlobalContext.ActiveTeam);

            if (VisibleMenu == MenuType.ActionMenu)
            {
                GenerateActionMenuDescription(windowColor);
            }
        }


        private void GenerateActionMenuDescription(Color windowColor)
        {
            ActionMenuDescriptionWindow = new Window(
                ContextMenuUtils.GetActionDescriptionForCurrentMenuOption(ActionMenuContext.CurrentMenu),
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
            EntityWindow = new AnimatedRenderable(GenerateEntityWindow(hoverSlice), RightSideWindowAnimation);
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
                                : RenderBlank.Blank
                        },
                        {
                            (hoverSlice.ItemEntity != null)
                                ? new Window(
                                    hoverSlice.ItemEntity.TerrainInfo,
                                    ItemTerrainWindowColor
                                ) as IRenderable
                                : RenderBlank.Blank
                        }
                    });
            }
            else
            {
                bool canMove = UnitMovingPhase.CanEndMoveAtCoordinates(hoverSlice.MapCoordinates);

                var noEntityContent = new WindowContentGrid(
                    new[,]
                    {
                        {
                            new RenderText(AssetManager.WindowFont,
                                $"[ X: {GlobalContext.MapCursor.MapCoordinates.X}, Y: {GlobalContext.MapCursor.MapCoordinates.Y} ]"),
                            RenderBlank.Blank
                        },
                        {
                            new RenderText(AssetManager.WindowFont, "None"),
                            RenderBlank.Blank
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
                    });
            }

            return new Window(terrainContentGrid, new Color(50, 50, 50, 150));
        }

        public void GenerateObjectiveWindow()
        {
            ObjectiveWindow = GlobalContext.Scenario.ScenarioInfo();
        }

        public void GenerateInitiativeWindow()
        {
            GenerateTeamInitiativeWindow(Team.Blue);
            GenerateTeamInitiativeWindow(Team.Red);

            InitiativeWindow = new Window(
                new WindowContentGrid(new IRenderable[,] {{BlueTeamWindow, RedTeamWindow}}),
                Color.Transparent,
                HorizontalAlignment.Centered
            );
        }

        private void GenerateTeamInitiativeWindow(Team team)
        {
            const int unitsPerRow = 10;
            const int initiativeHealthBarHeight = 10;

            List<GameUnit> unitList = GlobalContext.Units.FindAll(unit => unit.Team == team);

            int rows = Convert.ToInt32(Math.Ceiling((float) unitList.Count / unitsPerRow));

            var unitListGrid = new IRenderable[rows, unitsPerRow];

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
                        unitListGrid[row, column] = RenderBlank.Blank;
                    }

                    unitIndex++;
                }
            }

            var unitListContentGrid =
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
                        ? MiscIconProvider.GetMiscIcon(MiscIcon.Crown, new Vector2(crownIconSize))
                        : RenderBlank.Blank,
                    new RenderText(AssetManager.MapFont, unit.Id)
                },
                {
                    RenderBlank.Blank,
                    unit.SmallPortrait
                },
                {
                    RenderBlank.Blank,
                    unit.IsCommander
                        ? unit.GetInitiativeCommandPointBar(new Vector2(unit.SmallPortrait.Width,
                            (float) initiativeHealthBarHeight / 2))
                        : RenderBlank.Blank
                },
                {
                    RenderBlank.Blank,
                    unit.GetInitiativeHealthBar(new Vector2(unit.SmallPortrait.Width, initiativeHealthBarHeight))
                }
            };

            IRenderable singleUnitContent = new Window(
                new WindowContentGrid(unitContent, 3, HorizontalAlignment.Centered),
                windowColorOverride ?? TeamUtility.DetermineTeamWindowColor(unit.Team)
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
                Color windowColor = TeamUtility.DetermineTeamWindowColor(hoverMapUnit.Team);
                Window leftUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                Window leftUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                Window leftUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                Window leftUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);

                LeftUnitPortraitWindow = new AnimatedRenderable(leftUnitPortraitWindow, LeftSideWindowAnimation);
                LeftUnitDetailWindow = new AnimatedRenderable(leftUnitDetailWindow, LeftSideWindowAnimation);

                LeftUnitStatusWindow = leftUnitStatusWindow != null
                    ? new AnimatedRenderable(leftUnitStatusWindow, LeftSideWindowAnimation)
                    : null;
                LeftUnitInventoryWindow = leftUnitInventoryWindow != null
                    ? new AnimatedRenderable(leftUnitInventoryWindow, LeftSideWindowAnimation)
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
                Color windowColor = TeamUtility.DetermineTeamWindowColor(hoverMapUnit.Team);
                Window rightUnitPortraitWindow = GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                Window rightUnitDetailWindow = GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
                Window rightUnitStatusWindow = GenerateUnitStatusWindow(hoverMapUnit.StatusEffects, windowColor);
                Window rightUnitInventoryWindow = GenerateUnitInventoryWindow(hoverMapUnit.InventoryPane, windowColor);


                RightUnitPortraitWindow = new AnimatedRenderable(rightUnitPortraitWindow, RightSideWindowAnimation);
                RightUnitDetailWindow = new AnimatedRenderable(rightUnitDetailWindow, RightSideWindowAnimation);
                RightUnitStatusWindow = rightUnitStatusWindow != null
                    ? new AnimatedRenderable(rightUnitStatusWindow, RightSideWindowAnimation)
                    : null;
                RightUnitInventoryWindow = rightUnitInventoryWindow != null
                    ? new AnimatedRenderable(rightUnitInventoryWindow, RightSideWindowAnimation)
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


            var selectedUnitStatuses = new IRenderable[statusEffects.Count, 1];

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

            return new Window(new WindowContentGrid(selectedUnitStatuses), windowColor);
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
                GameDriver.ScreenSize.X / 3 - (float) ActionMenuContext.CurrentMenu.Width / 2,
                (GameDriver.ScreenSize.Y / 2) - ((float) ActionMenuContext.CurrentMenu.Height / 2)
            );
        }

        private Vector2 ActionMenuDescriptionPosition()
        {
            //Right of Action Menu
            return new Vector2(
                WindowEdgeBuffer + ActionMenuPosition().X + ActionMenuContext.CurrentMenu.Width,
                ActionMenuPosition().Y
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
            float verticalAnchor = (LeftUnitInventoryWindow == null)
                ? LeftUnitPortraitWindowPosition().Y
                : LeftUnitInventoryWindowPosition().Y;

            //Bottom-left, above portrait/inventory
            return new Vector2(
                LeftUnitPortraitWindowPosition().X,
                verticalAnchor - LeftUnitStatusWindow.Height - WindowEdgeBuffer
            );
        }


        private Vector2 LeftUnitInventoryWindowPosition()
        {
            //Bottom-left, above stats
            return new Vector2(
                LeftUnitDetailWindowPosition().X,
                LeftUnitDetailWindowPosition().Y - LeftUnitInventoryWindow.Height
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
            float verticalAnchor = (RightUnitInventoryWindow == null)
                ? RightUnitPortraitWindowPosition().Y
                : RightUnitInventoryWindowPosition().Y;

            //Bottom-right, above portrait/inventory
            return new Vector2(
                RightUnitPortraitWindowPosition().X + RightUnitPortraitWindow.Width - RightUnitStatusWindow.Width,
                verticalAnchor - RightUnitStatusWindow.Height - WindowEdgeBuffer
            );
        }


        private Vector2 RightUnitInventoryWindowPosition()
        {
            //Bottom-left, above stats
            return new Vector2(
                RightUnitDetailWindowPosition().X + RightUnitDetailWindow.Width - RightUnitInventoryWindow.Width,
                RightUnitDetailWindowPosition().Y - RightUnitInventoryWindow.Height
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

        private Vector2 GoldWindowPosition()
        {
            //Center, above initiative list
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) TeamInfoWindow.Width / 2,
                InitiativeWindowPosition().Y - TeamInfoWindow.Height
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
            return CenterItemOnScreen(TakeItemMenu);
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
            return menuPosition - new Vector2(0, MenuHeaderWindow.Height);
        }

        #endregion Window Positions

        public float Width => GameDriver.VirtualResolution.X;
        public float Height => GameDriver.VirtualResolution.Y;

        public void Update(GameTime gameTime)
        {
            //Do nothing
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EntityWindow?.Draw(spriteBatch, EntityWindowPosition());
            if (InitiativeWindow != null)
            {
                InitiativeWindow.Draw(spriteBatch, InitiativeWindowPosition());
                TeamInfoWindow?.Draw(spriteBatch, GoldWindowPosition());

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

            if (ActionMenuContext?.CurrentMenu != null)
            {
                ActionMenuContext.CurrentMenu.Draw(spriteBatch, ActionMenuPosition());

                if (ActionMenuContext.CurrentMenu.IsVisible)
                {
                    ActionMenuDescriptionWindow?.Draw(spriteBatch, ActionMenuDescriptionPosition());
                    MenuHeaderWindow?.Draw(spriteBatch, MenuDescriptionWindowPosition(ActionMenuPosition()));
                }
            }

            ObjectiveWindow?.Draw(spriteBatch, ObjectiveWindowPosition());
            AdHocDraftMenu?.Draw(spriteBatch, AdHocDraftMenuPosition());
            TakeItemMenu?.Draw(spriteBatch, SteamItemMenuPosition());
            CenterScreenContent?.Draw(spriteBatch, CenterItemOnScreen(CenterScreenContent));
        }
    }
}