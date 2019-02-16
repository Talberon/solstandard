using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.DraftMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class DraftView : IUserInterface
    {
        private static readonly Color DarkBackgroundColor = new Color(100, 100, 100, 225);

        private const int WindowPadding = 10;

        private Window BlueTeamUnits { get; set; }
        private Window RedTeamUnits { get; set; }

        private Window BlueTeamCommander { get; set; }
        private Window RedTeamCommander { get; set; }

        private Window HelpText { get; set; }
        private Window VersusText { get; set; }

        public TwoDimensionalMenu UnitSelect { get; private set; }
        public VerticalMenu CommanderSelect { get; private set; }

        private static IRenderable _draftCursor;
        private static IRenderable _commanderCursor;
        private readonly IRenderable background;

        private bool visible;

        public DraftView(IRenderable background)
        {
            this.background = background;

            //TODO Black Silhouette
            BlueTeamCommander = new Window(new RenderBlank(), Color.Transparent);
            RedTeamCommander = new Window(new RenderBlank(), Color.Transparent);

            HelpText = new Window(new RenderText(AssetManager.HeavyFont, "SELECT A UNIT"), Color.Transparent);
            VersusText = new Window(new RenderText(AssetManager.HeavyFont, "VS"), Color.Transparent);
        }

        public void UpdateTeamUnitsWindow(List<IRenderable> unitSprites, Team team)
        {
            switch (team)
            {
                case Team.Blue:
                    BlueTeamUnits = BuildUnitListWindow(unitSprites);
                    break;
                case Team.Red:
                    RedTeamUnits = BuildUnitListWindow(unitSprites);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("team", team, null);
            }
        }

        public void UpdateCommanderSelect(IEnumerable<GameUnit> units, Team team)
        {
            MenuOption[] unitCommanderOptions =
                units.Select(unit => new SelectCommanderOption(unit)).Cast<MenuOption>().ToArray();

            CommanderSelect = new VerticalMenu(unitCommanderOptions, CommanderCursor,
                TeamUtility.DetermineTeamColor(team));
        }

        public void ClearCommanderSelect()
        {
            CommanderSelect = null;
        }

        public void UpdateUnitSelectMenu(Team team)
        {
            UnitSelect = new TwoDimensionalMenu(GetUnitOptionsForTeam(team), DraftCursor, DarkBackgroundColor,
                TwoDimensionalMenu.CursorType.Frame);
        }

        private static IRenderable DraftCursor
        {
            get
            {
                return _draftCursor ?? (
                           _draftCursor = new SpriteAtlas(
                               AssetManager.MapCursorTexture,
                               new Vector2(GameDriver.CellSize),
                               new Vector2(150)
                           )
                       );
            }
        }

        private static IRenderable CommanderCursor
        {
            get
            {
                return _commanderCursor ?? (
                           _commanderCursor = new SpriteAtlas(
                               AssetManager.MenuCursorTexture,
                               new Vector2(AssetManager.MenuCursorTexture.Width)
                           )
                       );
            }
        }

        private static MenuOption[,] GetUnitOptionsForTeam(Team team)
        {
            return new MenuOption[,]
            {
                {
                    new DraftUnitOption(Role.Champion, team),
                    new DraftUnitOption(Role.Lancer, team),
                    new DraftUnitOption(Role.Archer, team),
                    new DraftUnitOption(Role.Mage, team),
                    new DraftUnitOption(Role.Bard, team)
                }
            };
        }

        private static Window BuildUnitListWindow(IReadOnlyList<IRenderable> unitSprites)
        {
            const int unitsPerRow = 4;

            int totalRows = (int) Math.Ceiling((float) unitSprites.Count / unitsPerRow);

            IRenderable[,] unitCells = new IRenderable[unitsPerRow, totalRows];

            int unitIndex = 0;

            for (int row = 0; row < totalRows; row++)
            {
                for (int column = 0; column < unitsPerRow; column++)
                {
                    if (unitIndex >= unitSprites.Count)
                    {
                        unitCells[column, row] = new RenderBlank();
                    }
                    else
                    {
                        unitCells[column, row] = unitSprites[unitIndex];
                    }

                    unitIndex++;
                }
            }

            WindowContentGrid unitGrid = new WindowContentGrid(unitCells, 1, HorizontalAlignment.Centered);

            return new Window(unitGrid, DarkBackgroundColor, HorizontalAlignment.Centered);
        }


        #region Positions

        private Vector2 UnitSelectPosition
        {
            //Hori-Center, Lower Third
            get
            {
                return new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 3 * 2) -
                       new Vector2((float) UnitSelect.Width / 2, (float) UnitSelect.Height / 2);
            }
        }

        private Vector2 CommanderSelectPosition
        {
            //Hori-Center, Lower Third
            get
            {
                return new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 3 * 2) -
                       new Vector2((float) UnitSelect.Width / 2, (float) UnitSelect.Height / 2);
            }
        }

        private Vector2 HelpTextPosition
        {
            //Hori-Center, Upper Third
            get
            {
                return new Vector2(GameDriver.ScreenSize.X / 2, GameDriver.ScreenSize.Y / 3) -
                       new Vector2((float) UnitSelect.Width / 2, (float) UnitSelect.Height / 2);
            }
        }

        private Vector2 VersusTextPosition
        {
            //Anchored above UnitSelect
            get
            {
                Vector2 unitSelectPosition = UnitSelectPosition;
                return new Vector2(
                    unitSelectPosition.X + ((float) UnitSelect.Width / 2),
                    unitSelectPosition.Y - VersusText.Height - WindowPadding
                );
            }
        }

        private Vector2 BlueTeamCommanderPosition
        {
            //Anchored Center-Left of VersusText
            get
            {
                Vector2 vsTextCenter = VersusTextPosition +
                                       (new Vector2(VersusText.Width, VersusText.Height) / 2);

                return new Vector2(
                    vsTextCenter.X - VersusText.Width - BlueTeamCommander.Width - WindowPadding,
                    vsTextCenter.Y - ((float) BlueTeamCommander.Height / 2)
                );
            }
        }

        private Vector2 BlueTeamUnitsPosition
        {
            //Anchored Upper-Left of BlueTeamCommander
            get
            {
                Vector2 blueTeamCommanderPosition = BlueTeamCommanderPosition;

                return new Vector2(
                    blueTeamCommanderPosition.X - BlueTeamUnits.Width - WindowPadding,
                    blueTeamCommanderPosition.Y - BlueTeamUnits.Height + BlueTeamCommander.Height
                );
            }
        }

        private Vector2 RedTeamCommanderPosition
        {
            //Anchored Center-Right of VersusText
            get
            {
                Vector2 vsTextCenter = VersusTextPosition +
                                       (new Vector2(VersusText.Width, VersusText.Height) / 2);

                return new Vector2(
                    vsTextCenter.X + VersusText.Width + WindowPadding,
                    vsTextCenter.Y - ((float) RedTeamCommander.Height / 2)
                );
            }
        }

        private Vector2 RedTeamUnitsPosition
        {
            //Anchored Upper-Right of RedTeamCommander
            get
            {
                Vector2 redTeamCommanderPosition = RedTeamCommanderPosition;

                return new Vector2(
                    redTeamCommanderPosition.X + RedTeamCommander.Width + WindowPadding,
                    redTeamCommanderPosition.Y - RedTeamUnits.Height + RedTeamCommander.Height
                );
            }
        }

        #endregion

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawBackground(spriteBatch);

            if (BlueTeamUnits != null) BlueTeamUnits.Draw(spriteBatch, BlueTeamUnitsPosition);
            if (RedTeamUnits != null) RedTeamUnits.Draw(spriteBatch, RedTeamUnitsPosition);

            if (BlueTeamCommander != null) BlueTeamCommander.Draw(spriteBatch, BlueTeamCommanderPosition);
            if (RedTeamCommander != null) RedTeamCommander.Draw(spriteBatch, RedTeamCommanderPosition);

            if (HelpText != null) HelpText.Draw(spriteBatch, HelpTextPosition);
            if (VersusText != null) VersusText.Draw(spriteBatch, VersusTextPosition);

            if (UnitSelect != null) UnitSelect.Draw(spriteBatch, UnitSelectPosition);
            if (CommanderSelect != null) CommanderSelect.Draw(spriteBatch, CommanderSelectPosition);
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, GameDriver.ScreenSize / 2 - backgroundCenter);
        }
    }
}