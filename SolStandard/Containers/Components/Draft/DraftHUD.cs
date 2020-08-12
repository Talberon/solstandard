using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Codex;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.DraftMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.Draft
{
    public class DraftHUD : IUserInterface
    {
        private static readonly Color DarkBackgroundColor = new Color(50, 50, 50, 180);

        private const int WindowPadding = 10;
        private const int PortraitSize = 128;

        private Window BlueTeamUnits { get; set; }
        private Window RedTeamUnits { get; set; }

        private Window BlueTeamCommander { get; set; }
        private Window RedTeamCommander { get; set; }

        private Window HelpText { get; set; }
        private Window ControlsText { get; set; }
        private Window VersusText { get; }
        private Window ObjectivesWindow { get; set; }

        public TwoDimensionalMenu UnitSelect { get; private set; }
        public TwoDimensionalMenu CommanderSelect { get; private set; }

        private static IRenderable _draftCursor;
        private static IRenderable _commanderCursor;

        public DraftHUD()
        {
            UpdateCommanderPortrait(Role.Silhouette, Team.Creep);

            UpdateHelpWindow("SELECT A UNIT");
            VersusText = new Window(new RenderText(AssetManager.HeavyFont, "VS"), Color.Transparent);
        }

        public void UpdateControlsTextWindow()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;
            int iconSize = Convert.ToInt32(windowFont.MeasureString("A").Y);

            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.HeaderFont, "~Draft Phase~"),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Move Draft Cursor: "),
                    InputIconProvider.GetInputIcon(Input.CursorUp, iconSize),
                    InputIconProvider.GetInputIcon(Input.CursorDown, iconSize),
                    InputIconProvider.GetInputIcon(Input.CursorLeft, iconSize),
                    InputIconProvider.GetInputIcon(Input.CursorRight, iconSize)
                },
                {
                    new RenderText(windowFont, "Draft a unit: "),
                    InputIconProvider.GetInputIcon(Input.Confirm, iconSize),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "View Unit Codex: "),
                    InputIconProvider.GetInputIcon(Input.PreviewUnit, iconSize),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Move Map Camera: "),
                    InputIconProvider.GetInputIcon(Input.CameraUp, iconSize),
                    InputIconProvider.GetInputIcon(Input.CameraDown, iconSize),
                    InputIconProvider.GetInputIcon(Input.CameraLeft, iconSize),
                    InputIconProvider.GetInputIcon(Input.CameraRight, iconSize)
                }
            };
            var promptWindowContentGrid =
                new WindowContentGrid(promptTextContent, 2, HorizontalAlignment.Right);

            ControlsText = new Window(promptWindowContentGrid, DarkBackgroundColor);
        }

        public void UpdateTeamUnitsWindow(List<IRenderable> unitSprites, Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    BlueTeamUnits = BuildUnitListWindow(unitSprites, team);
                    break;
                case Team.Red:
                    RedTeamUnits = BuildUnitListWindow(unitSprites, team);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }
        }

        public void UpdateObjectivesWindow(Window objectivesWindow)
        {
            ObjectivesWindow = objectivesWindow;
        }

        public void UpdateCommanderSelect(List<GameUnit> units, Team team)
        {
            MenuOption[,] commanderOptions = GetCommanderOptionsForUnits(units);
            CommanderSelect = new TwoDimensionalMenu(commanderOptions, CommanderCursor,
                TeamUtility.DetermineTeamWindowColor(team), TwoDimensionalMenu.CursorType.Pointer);
        }


        private static MenuOption[,] GetCommanderOptionsForUnits(IReadOnlyList<GameUnit> units)
        {
            const int unitsPerRow = 5;

            int totalRows = (int) Math.Ceiling((float) units.Count / unitsPerRow);

            var commanderOptions = new MenuOption[totalRows, unitsPerRow];

            int unitIndex = 0;
            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex < units.Count)
                    {
                        GameUnit currentUnit = units[unitIndex];

                        UnitAction commandAction = CodexContext.UnitArchetypes
                            .First(archetype => archetype.Role == currentUnit.Role)
                            .Actions
                            .FirstOrDefault(action => action is ICommandAction);

                        commanderOptions[row, column] = new SelectCommanderOption(currentUnit, commandAction);
                    }
                    else
                    {
                        commanderOptions[row, column] = new UnselectableOption(RenderBlank.Blank, DarkBackgroundColor);
                    }

                    unitIndex++;
                }
            }

            return commanderOptions;
        }

        public void HideUnitSelect()
        {
            UnitSelect.IsVisible = false;
        }

        public void HideCommanderSelect()
        {
            CommanderSelect.IsVisible = false;
        }

        public void UpdateHelpWindow(string message)
        {
            HelpText = new Window(new RenderText(AssetManager.HeavyFont, message), DarkBackgroundColor);
        }

        public void UpdateUnitSelectMenu(Team team, Dictionary<Role, bool> unitsExhausted)
        {
            UnitSelect = new TwoDimensionalMenu(GetUnitOptionsForTeam(team, unitsExhausted), DraftCursor,
                TeamUtility.DetermineTeamWindowColor(team), TwoDimensionalMenu.CursorType.Frame);
        }

        public void UpdateCommanderPortrait(Role role, Team team)
        {
            ITexture2D unitPortrait = UnitGenerator.GetUnitPortrait(role, team);
            var commanderPortrait =
                new SpriteAtlas(unitPortrait, new Vector2(unitPortrait.Width, unitPortrait.Height),
                    new Vector2(PortraitSize));

            switch (team)
            {
                case Team.Blue:
                    BlueTeamCommander = new Window(commanderPortrait, TeamUtility.DetermineTeamWindowColor(team));
                    break;
                case Team.Red:
                    RedTeamCommander = new Window(commanderPortrait, TeamUtility.DetermineTeamWindowColor(team));
                    break;
                case Team.Creep:
                    RedTeamCommander = new Window(commanderPortrait, TeamUtility.DetermineTeamWindowColor(Team.Red));
                    BlueTeamCommander = new Window(commanderPortrait, TeamUtility.DetermineTeamWindowColor(Team.Blue));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(team), team, null);
            }
        }

        public static IRenderable DraftCursor =>
            _draftCursor ??= new SpriteAtlas(
                AssetManager.MapCursorTexture,
                GameDriver.CellSizeVector,
                new Vector2(150)
            );

        private static IRenderable CommanderCursor =>
            _commanderCursor ??= new SpriteAtlas(
                AssetManager.MenuCursorTexture,
                new Vector2(AssetManager.MenuCursorTexture.Width)
            );

        public static MenuOption[,] GetAdHocUnitOptionsForTeam(Team team, IReadOnlyDictionary<Role, bool> unitEnabled)
        {
            const int unitsPerRow = 6;
            List<Role> availableRoles = DraftContext.AvailableRoles;

            int totalRows = (int) Math.Ceiling((float) availableRoles.Count / unitsPerRow);

            var options = new MenuOption[totalRows, unitsPerRow];

            int unitIndex = 0;
            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex < availableRoles.Count)
                    {
                        Role currentRole = availableRoles[unitIndex];
                        // ReSharper disable once SimplifyConditionalTernaryExpression
                        bool enabled = unitEnabled.ContainsKey(currentRole) ? unitEnabled[currentRole] : true;
                        options[row, column] = new AdHocDraftOption(currentRole, team, enabled);
                    }
                    else
                    {
                        options[row, column] = new AdHocDraftOption(Role.Silhouette, team, false);
                    }

                    unitIndex++;
                }
            }

            return options;
        }


        private static MenuOption[,] GetUnitOptionsForTeam(Team team, IReadOnlyDictionary<Role, bool> unitEnabled)
        {
            const int unitsPerRow = 6;
            List<Role> availableRoles = DraftContext.AvailableRoles;

            int totalRows = (int) Math.Ceiling((float) availableRoles.Count / unitsPerRow);

            var options = new MenuOption[totalRows, unitsPerRow];

            int unitIndex = 0;
            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex < availableRoles.Count)
                    {
                        Role currentRole = availableRoles[unitIndex];
                        // ReSharper disable once SimplifyConditionalTernaryExpression
                        bool enabled = unitEnabled.ContainsKey(currentRole) ? unitEnabled[currentRole] : true;
                        options[row, column] = new DraftUnitOption(currentRole, team, enabled);
                    }
                    else
                    {
                        options[row, column] = new DraftUnitOption(Role.Silhouette, team, false);
                    }

                    unitIndex++;
                }
            }

            return options;
        }

        private static Window BuildUnitListWindow(IReadOnlyList<IRenderable> unitSprites, Team team)
        {
            const int unitsPerRow = 4;

            int totalRows = (int) Math.Ceiling((float) unitSprites.Count / unitsPerRow);

            var unitCells = new IRenderable[totalRows, unitsPerRow];

            int unitIndex = 0;

            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex >= unitSprites.Count)
                    {
                        unitCells[row, column] = RenderBlank.Blank;
                    }
                    else
                    {
                        unitCells[row, column] = unitSprites[unitIndex];
                    }

                    unitIndex++;
                }
            }

            var unitGrid = new WindowContentGrid(unitCells, 1, HorizontalAlignment.Centered);

            return new Window(unitGrid, TeamUtility.DetermineTeamWindowColor(team), HorizontalAlignment.Centered);
        }


        #region Positions

        private Vector2 UnitSelectPosition =>
            new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 3 * 2) -
            new Vector2((float) UnitSelect.Width / 2, (float) UnitSelect.Height / 2);

        private Vector2 CommanderSelectPosition =>
            new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 3 * 2) -
            new Vector2((float) CommanderSelect.Width / 2, (float) CommanderSelect.Height / 2);

        private Vector2 HelpTextPosition =>
            new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 5) -
            new Vector2((float) HelpText.Width / 2, (float) HelpText.Height / 2);

        private Vector2 VersusTextPosition
        {
            //Anchored above UnitSelect
            get
            {
                const int extraPadding = 50;
                (float x, float y) = UnitSelectPosition;
                return new Vector2(
                    x + ((float) UnitSelect.Width / 2) - ((float) VersusText.Width / 2),
                    y - VersusText.Height - WindowPadding - extraPadding
                );
            }
        }

        private Vector2 BlueTeamCommanderPosition
        {
            //Anchored Center-Left of VersusText
            get
            {
                (float x, float y) = VersusTextPosition + (new Vector2(VersusText.Width, VersusText.Height) / 2);

                return new Vector2(
                    x - VersusText.Width - BlueTeamCommander.Width - WindowPadding,
                    y - ((float) BlueTeamCommander.Height / 2)
                );
            }
        }

        private Vector2 BlueTeamUnitsPosition
        {
            //Anchored Upper-Left of BlueTeamCommander
            get
            {
                (float x, float y) = BlueTeamCommanderPosition;

                return new Vector2(
                    x - BlueTeamUnits.Width - WindowPadding,
                    y - BlueTeamUnits.Height + BlueTeamCommander.Height
                );
            }
        }

        private Vector2 RedTeamCommanderPosition
        {
            //Anchored Center-Right of VersusText
            get
            {
                (float x, float y) = VersusTextPosition + (new Vector2(VersusText.Width, VersusText.Height) / 2);

                return new Vector2(
                    x + VersusText.Width + WindowPadding,
                    y - ((float) RedTeamCommander.Height / 2)
                );
            }
        }

        private Vector2 RedTeamUnitsPosition
        {
            //Anchored Upper-Right of RedTeamCommander
            get
            {
                (float x, float y) = RedTeamCommanderPosition;

                return new Vector2(
                    x + RedTeamCommander.Width + WindowPadding,
                    y - RedTeamUnits.Height + RedTeamCommander.Height
                );
            }
        }

        private Vector2 ControlsTextPosition =>
            new Vector2(
                GameDriver.ScreenSize.X - ControlsText.Width - WindowPadding,
                GameDriver.ScreenSize.Y - ControlsText.Height - WindowPadding
            );

        private Vector2 ObjectivesWindowPosition =>
            new Vector2(
                WindowPadding,
                GameDriver.ScreenSize.Y - ObjectivesWindow.Height - WindowPadding
            );

        #endregion Positions

        public void Draw(SpriteBatch spriteBatch)
        {
            BlueTeamUnits?.Draw(spriteBatch, BlueTeamUnitsPosition);
            RedTeamUnits?.Draw(spriteBatch, RedTeamUnitsPosition);

            BlueTeamCommander?.Draw(spriteBatch, BlueTeamCommanderPosition);
            RedTeamCommander?.Draw(spriteBatch, RedTeamCommanderPosition);

            HelpText?.Draw(spriteBatch, HelpTextPosition);
            VersusText?.Draw(spriteBatch, VersusTextPosition);

            UnitSelect?.Draw(spriteBatch, UnitSelectPosition);
            CommanderSelect?.Draw(spriteBatch, CommanderSelectPosition);

            ControlsText?.Draw(spriteBatch, ControlsTextPosition);
            ObjectivesWindow?.Draw(spriteBatch, ObjectivesWindowPosition);
        }
    }
}