using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.Entity.Unit.Actions;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.CodexMenu;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class CodexView : IUserInterface
    {
        public static readonly Color CodexWindowColor = new Color(50, 50, 50, 180);
        public readonly TwoDimensionalMenu UnitListMenu;
        private Window unitActionListWindow;
        private Window unitDetailWindow;
        private readonly SpriteAtlas background;

        private const int WindowEdgeBuffer = 10;

        private static IRenderable _codexCursor;

        private bool visible;

        public CodexView(IReadOnlyList<GameUnit> unitArchetypes)
        {
            UnitListMenu = BuildUnitMenu(unitArchetypes);
            visible = true;
            background = new SpriteAtlas(AssetManager.MainMenuBackground,
                new Vector2(AssetManager.MainMenuBackground.Width, AssetManager.MainMenuBackground.Height),
                GameDriver.ScreenSize);
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        #region Window Generation

        public void ShowUnitDetails(GameUnit unit)
        {
            Color windowColor = TeamUtility.DetermineTeamColor(unit.Team);
            unitActionListWindow = GenerateActionWindow(unit.Actions, windowColor);
            unitDetailWindow = GenerateUnitDetailWindow(unit);
        }

        private static Window GenerateActionWindow(IReadOnlyList<UnitAction> actions, Color windowColor)
        {
            IRenderable[,] actionElements = new IRenderable[actions.Count, 4];

            const int iconIndex = 0;
            const int nameIndex = 1;
            const int rangeIndex = 2;
            const int descriptionIndex = 3;

            int largestNameWidth = 0;
            int largestRangeWidth = 0;
            int largestDescriptionWidth = 0;

            for (int i = 0; i < actions.Count; i++)
            {
                actionElements[i, iconIndex] = actions[i].Icon;

                actionElements[i, nameIndex] =
                    new Window(new RenderText(AssetManager.WindowFont, actions[i].Name), Color.Transparent);

                actionElements[i, rangeIndex] = new Window(
                    new RenderText(
                        AssetManager.WindowFont,
                        actions[i].Range == null
                            ? "Range: N/A"
                            : string.Format("Range: [{0}]", string.Join(",", actions[i].Range))
                    ),
                    Color.Transparent
                );

                actionElements[i, descriptionIndex] =
                    new Window(new RenderText(AssetManager.WindowFont, actions[i].Description), windowColor);

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


            for (int i = 0; i < actions.Count; i++)
            {
                //Fill space so that all the elements have the same width like a grid
                ((Window) actionElements[i, nameIndex]).Width = largestNameWidth;
                ((Window) actionElements[i, rangeIndex]).Width = largestRangeWidth;
                ((Window) actionElements[i, descriptionIndex]).Width = largestDescriptionWidth;
            }

            Window skillTable = new Window(new WindowContentGrid(actionElements, 5), windowColor);


            return new Window(new WindowContentGrid(new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.HeaderFont, "___Unit Skills___"),
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
            IRenderable unitPortrait = unit.UnitPortraitPane;
            IRenderable unitStats = unit.DetailPane;

            IRenderable[,] windowContent =
            {
                {
                    unitPortrait,
                    unitStats,
                    unitSprite
                }
            };

            Color windowColor = TeamUtility.DetermineTeamColor(unit.Team);

            return new Window(new WindowContentGrid(windowContent, 2), windowColor);
        }


        private static TwoDimensionalMenu BuildUnitMenu(IReadOnlyList<GameUnit> units)
        {
            MenuOption[,] menuOptions = new MenuOption[1, units.Count];

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

        private static IRenderable CodexCursor
        {
            get
            {
                return _codexCursor ?? (
                           _codexCursor = new SpriteAtlas(
                               AssetManager.MapCursorTexture,
                               new Vector2(GameDriver.CellSize),
                               new Vector2(150)
                           )
                       );
            }
        }

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
            Vector2 centerScreen = GameDriver.ScreenSize / 2;
            Vector2 backgroundCenter = new Vector2(background.Width, background.Height) / 2;
            background.Draw(spriteBatch, centerScreen - backgroundCenter);

            if (UnitListMenu != null) UnitListMenu.Draw(spriteBatch, UnitListMenuPosition());

            if (unitDetailWindow != null)
            {
                unitDetailWindow.Draw(spriteBatch, UnitDetailWindowPosition());

                if (unitActionListWindow != null)
                {
                    unitActionListWindow.Draw(spriteBatch, UnitActionListWindowPosition());
                }
            }
        }
    }
}