using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.CodexMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.Codex
{
    public class CodexView : IUserInterface
    {
        public static readonly Color CodexWindowColor = new Color(50, 50, 50, 180);
        public readonly TwoDimensionalMenu UnitListMenu;
        private Window unitActionListWindow;
        private Window unitDetailWindow;

        private const int WindowEdgeBuffer = 10;

        private static IRenderable _codexCursor;


        public CodexView(IReadOnlyList<GameUnit> unitArchetypes)
        {
            UnitListMenu = BuildUnitMenu(unitArchetypes);
        }

        #region Window Generation

        public void ShowUnitDetails(GameUnit unit)
        {
            Color windowColor = TeamUtility.DetermineTeamWindowColor(unit.Team);
            unitActionListWindow = GenerateActionWindow(unit.Actions, windowColor);
            unitDetailWindow = GenerateUnitDetailWindow(unit);
        }

        private static Window GenerateActionWindow(IEnumerable<UnitAction> unitActions, Color windowColor)
        {
            List<UnitAction> codexActions = new List<UnitAction>(unitActions)
                .Where(action => !(action is BasicAttack) && !(action is Wait))
                .ToList();

            var actionElements = new IRenderable[codexActions.Count, 4];

            const int iconIndex = 0;
            const int nameIndex = 1;
            const int rangeIndex = 2;
            const int descriptionIndex = 3;

            int largestNameWidth = 0;
            int largestRangeWidth = 0;
            int largestDescriptionWidth = 0;

            for (int i = 0; i < codexActions.Count; i++)
            {
                actionElements[i, iconIndex] = codexActions[i].Icon;

                actionElements[i, nameIndex] =
                    new Window(
                        new RenderText(AssetManager.WindowFont, codexActions[i].Name,
                            (codexActions[i].FreeAction) ? GlobalContext.PositiveColor : Color.White), Color.Transparent);

                actionElements[i, rangeIndex] = new Window(
                    new RenderText(
                        AssetManager.WindowFont,
                        codexActions[i].Range == null
                            ? "Range: N/A"
                            : $"Range: [{string.Join(",", codexActions[i].Range)}]"
                    ),
                    Color.Transparent
                );

                actionElements[i, descriptionIndex] =
                    new Window(codexActions[i].Description, windowColor);

                //Remember the largest width for aligning later
                if (actionElements[i, nameIndex].Width > largestNameWidth)
                {
                    largestNameWidth = actionElements[i, nameIndex].Width;
                }

                if (actionElements[i, rangeIndex].Width > largestRangeWidth)
                {
                    largestRangeWidth = actionElements[i, rangeIndex].Width;
                }

                if (actionElements[i, descriptionIndex].Width > largestDescriptionWidth)
                {
                    largestDescriptionWidth = actionElements[i, descriptionIndex].Width;
                }
            }


            for (int i = 0; i < codexActions.Count; i++)
            {
                //Fill space so that all the elements have the same width like a grid
                ((Window) actionElements[i, nameIndex]).Width = largestNameWidth;
                ((Window) actionElements[i, rangeIndex]).Width = largestRangeWidth;
                ((Window) actionElements[i, descriptionIndex]).Width = largestDescriptionWidth;
            }

            var skillTable = new Window(new WindowContentGrid(actionElements, 5), windowColor);


            return new Window(new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.HeaderFont, "___Unit Skills___")
                    },
                    {
                        skillTable
                    }
                },
                3,
                HorizontalAlignment.Centered
            ), windowColor);
        }

        private static Window GenerateUnitDetailWindow(GameUnit unit)
        {
            const int unitSpriteSize = 128;
            IRenderable unitSprite = unit.UnitEntity.UnitSpriteSheet.Resize(new Vector2(unitSpriteSize));
            IRenderable unitPortrait = unit.MediumPortrait;
            IRenderable unitStats = unit.DetailPane;

            IRenderable[,] windowContent =
            {
                {
                    unitPortrait,
                    unitStats,
                    unitSprite
                }
            };

            Color windowColor = TeamUtility.DetermineTeamWindowColor(unit.Team);

            return new Window(new WindowContentGrid(windowContent, 2), windowColor);
        }


        private static TwoDimensionalMenu BuildUnitMenu(IReadOnlyList<GameUnit> units)
        {
            var menuOptions = new MenuOption[1, units.Count];

            for (int i = 0; i < units.Count; i++)
            {
                menuOptions[0, i] = new UnitCodexOption(units[i]);
            }

            return new TwoDimensionalMenu(
                menuOptions,
                CodexCursor,
                CodexWindowColor,
                TwoDimensionalMenu.CursorType.Frame
            );
        }

        private static IRenderable CodexCursor =>
            _codexCursor ??= new SpriteAtlas(
                AssetManager.MapCursorTexture,
                GameDriver.CellSizeVector,
                new Vector2(150)
            );

        #endregion

        #region Positions

        private Vector2 UnitListMenuPosition()
        {
            return new Vector2(
                (GameDriver.ScreenSize.X / 2) - ((float) UnitListMenu.Width / 2),
                WindowEdgeBuffer
            );
        }

        private Vector2 UnitDetailWindowPosition()
        {
            return new Vector2(
                (GameDriver.ScreenSize.X / 2) - ((float) unitDetailWindow.Width / 2),
                UnitListMenuPosition().Y + UnitListMenu.Height + WindowEdgeBuffer
            );
        }

        private Vector2 UnitActionListWindowPosition()
        {
            return new Vector2(
                (GameDriver.ScreenSize.X / 2) - ((float) unitActionListWindow.Width / 2),
                UnitDetailWindowPosition().Y + unitDetailWindow.Height + WindowEdgeBuffer
            );
        }

        #endregion Positions

        

        public void Draw(SpriteBatch spriteBatch)
        {
            UnitListMenu?.Draw(spriteBatch, UnitListMenuPosition());

            if (unitDetailWindow == null) return;
            unitDetailWindow.Draw(spriteBatch, UnitDetailWindowPosition());
            unitActionListWindow?.Draw(spriteBatch, UnitActionListWindowPosition());
        }
    }
}