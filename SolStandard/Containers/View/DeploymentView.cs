using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.View
{
    public class DeploymentView : IUserInterface
    {
        private static readonly Color DarkBackgroundColor = new Color(50, 50, 50, 180);

        private const int WindowEdgePadding = 10;
        private bool visible;

        private Window BlueDeployRoster { get; set; }
        private Window RedDeployRoster { get; set; }

        private Window HelpText { get; set; }

        public DeploymentView(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit)
        {
            visible = true;

            UpdateRosterLists(blueArmy, redArmy, currentUnit);

            HelpText = GenerateHelpTextWindow();
        }


        private static Window GenerateHelpTextWindow()
        {
            ISpriteFont windowFont = AssetManager.WindowFont;

            IRenderable[,] promptTextContent =
            {
                {
                    new RenderText(windowFont, "Press "),
                    ButtonIconProvider.GetButton(ButtonIcon.A, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " on a deployment tile to deploy a unit."),
                    new RenderBlank(),
                    new RenderBlank()
                },
                {
                    new RenderText(windowFont, "Press "),
                    ButtonIconProvider.GetButton(ButtonIcon.Lb, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " or "),
                    ButtonIconProvider.GetButton(ButtonIcon.Rb, new Vector2(windowFont.MeasureString("A").Y)),
                    new RenderText(windowFont, " to cycle between units.")
                }
            };
            WindowContentGrid promptWindowContentGrid =
                new WindowContentGrid(promptTextContent, 2, HorizontalAlignment.Right);

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
                    units[0, i] = GameMapView.SingleUnitContent(unitList[i], hpBarHeight, Color.Yellow);
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
                //Top-Right
                return new Vector2(GameDriver.ScreenSize.X - HelpText.Width - WindowEdgePadding, WindowEdgePadding);
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
            //TODO Set positions for windows
            if (BlueDeployRoster != null) BlueDeployRoster.Draw(spriteBatch, BlueDeployRosterPosition);
            if (RedDeployRoster != null) RedDeployRoster.Draw(spriteBatch, RedDeployRosterPosition);
            if (HelpText != null) HelpText.Draw(spriteBatch, HelpTextPosition);
        }
    }
}