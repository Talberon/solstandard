using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.World;
using SolStandard.Entity;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Map.Elements.Cursor;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using SolStandard.Utility.Monogame;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.Deployment
{
    public class DeploymentHUD : IUserInterface
    {
        private const int WindowEdgePadding = 10;
        private static readonly Color DarkBackgroundColor = new Color(50, 50, 50, 180);
        private static readonly Color HighlightColor = new Color(135, 125, 13);

        private Window ItemDetailWindow { get; set; }
        private Window ObjectiveWindow { get; }
        private Window BlueDeployRoster { get; set; }
        private Window RedDeployRoster { get; set; }
        private Window UnitPortraitWindow { get; set; }
        private Window UnitDetailWindow { get; set; }
        private Window EntityWindow { get; set; }
        private Window HelpText { get; }

        public DeploymentHUD(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit, Scenario.Scenario scenario)
        {
            UpdateRosterLists(blueArmy, redArmy, currentUnit);
            ObjectiveWindow = scenario.ScenarioInfo(HorizontalAlignment.Centered);
            HelpText = GenerateHelpTextWindow();
        }

        public void SetEntityWindow(MapSlice hoverSlice)
        {
            EntityWindow = WorldHUD.GenerateEntityWindow(hoverSlice);
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
                Color windowColor = TeamUtility.DetermineTeamWindowColor(hoverMapUnit.Team);
                UnitPortraitWindow = WorldHUD.GenerateUnitPortraitWindow(hoverMapUnit.UnitPortraitPane, windowColor);
                UnitDetailWindow = WorldHUD.GenerateUnitDetailWindow(hoverMapUnit.DetailPane, windowColor);
            }
        }

        private static Window GenerateHelpTextWindow()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;

            int iconSize = Convert.ToInt32(windowFont.MeasureString("A").Y);

            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(AssetManager.HeaderFont, "Deployment Phase"),
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.Confirm, iconSize),
                    new RenderText(windowFont, " on a deployment tile to deploy a unit."),
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.Cancel, iconSize),
                    new RenderText(windowFont, " to snap to the first deploy tile."),
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.PreviewUnit, iconSize),
                    new RenderText(windowFont, " to preview selected unit in the codex."),
                    RenderBlank.Blank,
                    RenderBlank.Blank
                },
                {
                    new RenderText(windowFont, "Press "),
                    InputIconProvider.GetInputIcon(Input.TabLeft, iconSize),
                    new RenderText(windowFont, " or "),
                    InputIconProvider.GetInputIcon(Input.TabRight, iconSize),
                    new RenderText(windowFont, " to cycle between units.")
                }
            };
            var promptWindowContentGrid = new WindowContentGrid(promptTextContent, 2);

            return new Window(promptWindowContentGrid, DarkBackgroundColor);
        }

        public void UpdateRosterLists(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit)
        {
            BlueDeployRoster = BuildRosterList(blueArmy, currentUnit);
            RedDeployRoster = BuildRosterList(redArmy, currentUnit);
        }

        private static Window BuildRosterList(IReadOnlyList<GameUnit> unitList, GameUnit currentUnit)
        {
            var units = new IRenderable[1, unitList.Count];

            for (int i = 0; i < unitList.Count; i++)
            {
                const int hpBarHeight = 5;

                if (unitList[i] == currentUnit)
                {
                    units[0, i] = WorldHUD.SingleUnitContent(unitList[i], hpBarHeight, HighlightColor);
                }
                else
                {
                    units[0, i] = WorldHUD.SingleUnitContent(unitList[i], hpBarHeight, null);
                }
            }

            if (unitList.Count <= 0) return null;


            IRenderable unitContentGrid = new WindowContentGrid(units, 1, HorizontalAlignment.Centered);
            Color windowColor = TeamUtility.DetermineTeamWindowColor(unitList.First().Team);
            return new Window(unitContentGrid, windowColor);
        }

        public void GenerateItemDetailWindow(List<IItem> items, Color color)
        {
            ItemDetailWindow = WorldHUD.GenerateItemsWindow(items, color);
        }

        public void CloseItemDetailWindow()
        {
            ItemDetailWindow = null;
        }

        #region WindowPositions

        private Vector2 BlueDeployRosterPosition => new Vector2(WindowEdgePadding);

        private Vector2 RedDeployRosterPosition =>
            GameDriver.ScreenSize - new Vector2(WindowEdgePadding) -
            new Vector2(RedDeployRoster.Width, RedDeployRoster.Height);

        private Vector2 HelpTextPosition => new Vector2(WindowEdgePadding,
            GameDriver.ScreenSize.Y - HelpText.Height - WindowEdgePadding);

        private Vector2 EntityWindowPosition =>
            new Vector2(GameDriver.ScreenSize.X - EntityWindow.Width - WindowEdgePadding, WindowEdgePadding);

        private Vector2 UnitPortraitWindowPosition =>
            new Vector2(WindowEdgePadding,
                HelpTextPosition.Y - UnitPortraitWindow.Height - WindowEdgePadding
            );

        private Vector2 UnitDetailWindowPosition =>
            new Vector2(
                WindowEdgePadding + UnitPortraitWindow.Width,
                UnitPortraitWindowPosition.Y + UnitPortraitWindow.Height - UnitDetailWindow.Height
            );

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

        private Vector2 ObjectiveWindowPosition =>
            new Vector2(
                (GameDriver.ScreenSize.X / 2) - ((float) ObjectiveWindow.Width / 2),
                WindowEdgePadding
            );

        #endregion

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