using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts.WinConditions;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.View
{
    public class DeploymentView : IUserInterface
    {
        private static readonly Color DarkBackgroundColor = new Color(50, 50, 50, 180);
        private static readonly Color HighlightColor = new Color(135, 125, 13);

        private const int WindowEdgePadding = 10;
        private bool visible;

        private Window BlueDeployRoster { get; set; }
        private Window RedDeployRoster { get; set; }

        private Window UnitPortraitWindow { get; set; }
        private Window UnitDetailWindow { get; set; }

        public Window ItemDetailWindow { get; private set; }

        public Window ObjectiveWindow { get; private set; }

        private Window EntityWindow { get; set; }

        private Window HelpText { get; set; }

        public DeploymentView(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit, Scenario scenario)
        {
            visible = true;

            UpdateRosterLists(blueArmy, redArmy, currentUnit);

            ObjectiveWindow = scenario.ScenarioInfo(HorizontalAlignment.Centered);

            HelpText = GenerateHelpTextWindow();
        }

        public void SetEntityWindow(MapSlice hoverSlice)
        {
            EntityWindow = GameMapView.GenerateEntityWindow(hoverSlice);
        }

        public void UpdateHoverUnitWindows(GameUnit hoverMapUnit)
        {
            if (hoverMapUnit == null)
            {
                UnitPortraitWindow = null;
                UnitDetailWindow = null;
            }
            else
            {
                Color windowColor = TeamUtility.DetermineTeamColor(hoverMapUnit.Team);
                UnitPortraitWindow = GameMapView.GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                UnitDetailWindow = GameMapView.GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
            }
        }

        private static Window GenerateHelpTextWindow()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;

            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.HeaderFont, "Deployment Phase"),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.Confirm, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " on a deployment tile to deploy a unit."),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.Cancel, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " to snap to the first deploy tile."),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.PreviewUnit, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " to preview selected unit in the codex."),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.TabLeft, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " or "),
                    InputIconProvider.GetInputIcon(Input.TabRight, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " to cycle between units.")
                }
            };
            WindowContentGrid promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);

            return new Window(promptWindowContentGrid, DarkBackgroundColor);
        }

        public void UpdateRosterLists(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit)
        {
            BlueDeployRoster = BuildRosterList(blueArmy, currentUnit);
            RedDeployRoster = BuildRosterList(redArmy, currentUnit);
        }

        private static Window BuildRosterList(IReadOnlyList<GameUnit> unitList, GameUnit currentUnit)
        {
            IRenderable[,] units = new IRenderable[1, unitList.Count];

            for (int i = 0; i < unitList.Count; i++)
            {
                const int hpBarHeight = 5;

                if (unitList[i] == currentUnit)
                {
                    units[0, i] = GameMapView.SingleUnitContent(unitList[i], hpBarHeight, HighlightColor);
                }
                else
                {
                    units[0, i] = GameMapView.SingleUnitContent(unitList[i], hpBarHeight, null);
                }
            }

            if (unitList.Count <= 0) return null;


            IRenderable unitContentGrid = new WindowContentGrid(units, 1, HorizontalAlignment.Centered);
            Color windowColor = TeamUtility.DetermineTeamColor(unitList.First().Team);
            return new Window(unitContentGrid, windowColor);
        }

        public void GenerateItemDetailWindow(List<IItem> items, Color color)
        {
            ItemDetailWindow = GameMapView.GenerateItemsWindow(items, color);
        }

        public void CloseItemDetailWindow()
        {
            ItemDetailWindow = null;
        }

        #region WindowPositions

        private Vector2 BlueDeployRosterPosition
        {
            //Top-left
            get { return new Vector2(WindowEdgePadding); }
        }

        private Vector2 RedDeployRosterPosition
        {
            get
            {
                //Bottom-right
                return GameDriver.ScreenSize - new Vector2(WindowEdgePadding) -
                       new Vector2(RedDeployRoster.Width, RedDeployRoster.Height);
            }
        }

        private Vector2 HelpTextPosition
        {
            get
            {
                //Bottom-left
                return new Vector2(WindowEdgePadding, GameDriver.ScreenSize.Y - HelpText.Height - WindowEdgePadding);
            }
        }

        private Vector2 EntityWindowPosition
        {
            get
            {
                //Top-Right
                return new Vector2(GameDriver.ScreenSize.X - EntityWindow.Width - WindowEdgePadding, WindowEdgePadding);
            }
        }

        private Vector2 UnitPortraitWindowPosition
        {
            get
            {
                //Bottom-left, above HelpText window
                return new Vector2(WindowEdgePadding,
                    HelpTextPosition.Y - UnitPortraitWindow.Height - WindowEdgePadding
                );
            }
        }

        private Vector2 UnitDetailWindowPosition
        {
            get
            {
                //Bottom-left, right of portrait, above initiative window
                return new Vector2(
                    WindowEdgePadding + UnitPortraitWindow.Width,
                    UnitPortraitWindowPosition.Y + UnitPortraitWindow.Height - UnitDetailWindow.Height
                );
            }
        }

        private Vector2 ItemDetailWindowPosition
        {
            get
            {
                if (UnitDetailWindow == null || UnitPortraitWindow == null)
                {
                    //Bottom-left, above HelpText window
                    return new Vector2(WindowEdgePadding,
                        HelpTextPosition.Y - ItemDetailWindow.Height - WindowEdgePadding
                    );
                }

                //Bottom-left, above portrait
                return new Vector2(
                    UnitPortraitWindowPosition.X,
                    UnitDetailWindowPosition.Y - ItemDetailWindow.Height - WindowEdgePadding
                );
            }
        }

        private Vector2 ObjectiveWindowPosition
        {
            get
            {
                return new Vector2(
                    (GameDriver.ScreenSize.X / 2) - ((float) ObjectiveWindow.Width / 2),
                    WindowEdgePadding
                );
            }
        }

        #endregion

        //Show current unit being deployed
        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ObjectiveWindow?.Draw(spriteBatch, ObjectiveWindowPosition);

            BlueDeployRoster?.Draw(spriteBatch, BlueDeployRosterPosition);
            RedDeployRoster?.Draw(spriteBatch, RedDeployRosterPosition);
            HelpText?.Draw(spriteBatch, HelpTextPosition);
            EntityWindow?.Draw(spriteBatch, EntityWindowPosition);

            UnitPortraitWindow?.Draw(spriteBatch, UnitPortraitWindowPosition);
            UnitDetailWindow?.Draw(spriteBatch, UnitDetailWindowPosition);

            ItemDetailWindow?.Draw(spriteBatch, ItemDetailWindowPosition);
        }
    }
}