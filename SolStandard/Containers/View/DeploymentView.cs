using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;

namespace SolStandard.Containers.View
{
    public class DeploymentView : IUserInterface
    {
        private const int WindowEdgePadding = 10;
        private bool visible;

        private Window BlueDeployRoster { get; set; }
        private Window RedDeployRoster { get; set; }

        public DeploymentView(List<GameUnit> blueArmy, List<GameUnit> redArmy, GameUnit currentUnit)
        {
            visible = true;

            UpdateRosterLists(blueArmy, redArmy, currentUnit);
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
        }
    }
}