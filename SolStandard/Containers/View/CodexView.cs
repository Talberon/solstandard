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
        public static readonly Color CodexWindowColor = new Color(50, 50, 80, 200);
        public readonly TwoDimensionalMenu UnitListMenu;
        private Window unitActionListWindow;
        private Window unitDetailWindow;

        private const int WindowEdgeBuffer = 10;

        private static IRenderable _codexCursor;

        private bool visible;

        public CodexView(IReadOnlyList<GameUnit> unitArchetypes)
        {
            UnitListMenu = BuildUnitMenu(unitArchetypes);
            visible = true;
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        #region Window Generation

        public void ShowUnitDetails(GameUnit unit)
        {
            unitActionListWindow = GenerateActionWindow(unit.Actions);
            unitDetailWindow = GenerateUnitDetailWindow(unit);
        }

        private static Window GenerateActionWindow(IReadOnlyList<UnitAction> actions)
        {
            IRenderable[,] actionElements = new IRenderable[actions.Count, 3];

            int largestNameWidth = 0;
            int largestRangeWidth = 0;
            int largestDescriptionWidth = 0;

            for (int i = 0; i < actions.Count; i++)
            {
                actionElements[i, 0] =
                    new Window(new RenderText(AssetManager.WindowFont, actions[i].Name), CodexWindowColor);

                actionElements[i, 1] = new Window(
                    new RenderText(
                        AssetManager.WindowFont,
                        actions[i].Range == null
                            ? "N/A"
                            : string.Format("Range: [{0}]", string.Join(",", actions[i].Range))
                    ),
                    CodexWindowColor
                );

                actionElements[i, 2] = new Window(new RenderText(AssetManager.WindowFont, actions[i].Description),
                    CodexWindowColor);

                //Remember the largest width for aligning later
                if (actionElements[i, 0].Width > largestNameWidth)
                {
                    largestNameWidth = actionElements[i, 0].Width;
                }

                if (actionElements[i, 1].Width > largestRangeWidth)
                {
                    largestRangeWidth = actionElements[i, 1].Width;
                }

                if (actionElements[i, 2].Width > largestDescriptionWidth)
                {
                    largestDescriptionWidth = actionElements[i, 2].Width;
                }
            }


            for (int i = 0; i < actions.Count; i++)
            {
                //Fill space so that all the elements have the same width like a grid
                ((Window) actionElements[i, 0]).Width = largestNameWidth;
                ((Window) actionElements[i, 1]).Width = largestRangeWidth;
                ((Window) actionElements[i, 2]).Width = largestDescriptionWidth;
            }

            Window skillTable = new Window(new WindowContentGrid(actionElements, 5), CodexWindowColor);


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
            ), CodexWindowColor);
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

            return new Window(new WindowContentGrid(windowContent, 2), CodexWindowColor);
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