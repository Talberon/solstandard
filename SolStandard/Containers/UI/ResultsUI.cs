using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    public class ResultsUI : IUserInterface
    {
        private const int WindowEdgeBuffer = 5;
        private const int WindowPadding = 10;

        private Window BlueTeamLeaderPortraitWindow { get; set; }
        private Window BlueTeamUnitRosterWindow { get; set; }
        private Window BlueTeamResultWindow { get; set; }

        private Window VersusWindow { get; set; }

        private Window RedTeamLeaderPortraitWindow { get; set; }
        private Window RedTeamUnitRosterWindow { get; set; }
        private Window RedTeamResultWindow { get; set; }

        private readonly ITexture2D windowTexture;

        public bool Visible { get; private set; }

        public ResultsUI(ITexture2D windowTexture)
        {
            this.windowTexture = windowTexture;
        }

        public void UpdateWindows()
        {
            GenerateBlueTeamLeaderPortraitWindow();
            GenerateBlueTeamUnitRosterWindow();
            GenerateBlueTeamResultWindow("FIGHT!"); //TODO Make this dynamic

            GenerateRedTeamLeaderPortraitWindow();
            GenerateRedTeamUnitRosterWindow();
            GenerateRedTeamResultWindow("FIGHT!"); //TODO Make this dynamic

            GenerateVersusWindow();
        }

        #region Generation

        private void GenerateVersusWindow()
        {
            VersusWindow = new Window(
                "Versus Window",
                windowTexture,
                new RenderText(GameDriver.WindowFont, "VS"),
                new Color(50, 50, 50)
            );
        }

        private void GenerateBlueTeamLeaderPortraitWindow()
        {
            IRenderable[,] blueLeaderContent = {{FindTeamLeader(Team.Blue, Role.Monarch).LargePortrait}};

            BlueTeamLeaderPortraitWindow = new Window(
                "Blue Leader Portrait Window",
                windowTexture,
                new WindowContentGrid(blueLeaderContent, 1),
                TeamUtility.DetermineTeamColor(Team.Blue)
            );
        }

        private void GenerateBlueTeamUnitRosterWindow()
        {
            BlueTeamUnitRosterWindow = new Window(
                "Blue Team Roster",
                windowTexture,
                new WindowContentGrid(GenerateUnitRoster(Team.Blue), 2),
                TeamUtility.DetermineTeamColor(Team.Blue)
            );
        }

        private void GenerateBlueTeamResultWindow(string windowText)
        {
            BlueTeamResultWindow = new Window(
                "Blue Team Result Window",
                windowTexture,
                new RenderText(GameDriver.WindowFont, windowText),
                TeamUtility.DetermineTeamColor(Team.Blue)
            );
        }

        private void GenerateRedTeamLeaderPortraitWindow()
        {
            IRenderable[,] blueLeaderContent = {{FindTeamLeader(Team.Red, Role.Monarch).LargePortrait}};

            RedTeamLeaderPortraitWindow = new Window(
                "Red Leader Portrait Window",
                windowTexture,
                new WindowContentGrid(blueLeaderContent, 1),
                TeamUtility.DetermineTeamColor(Team.Red)
            );
        }

        private void GenerateRedTeamUnitRosterWindow()
        {
            RedTeamUnitRosterWindow = new Window(
                "Red Team Roster",
                windowTexture,
                new WindowContentGrid(GenerateUnitRoster(Team.Red), 2),
                TeamUtility.DetermineTeamColor(Team.Red)
            );
        }

        private void GenerateRedTeamResultWindow(string windowText)
        {
            RedTeamResultWindow = new Window(
                "Red Team Result Window",
                windowTexture,
                new RenderText(GameDriver.WindowFont, windowText),
                TeamUtility.DetermineTeamColor(Team.Red)
            );
        }


        private static GameUnit FindTeamLeader(Team team, Role role)
        {
            return GameContext.Units.Find(unit => unit.Team == team && unit.Role == role);
        }


        private IRenderable[,] GenerateUnitRoster(Team team)
        {
            List<GameUnit> teamUnits = GameContext.Units.FindAll(unit => unit.Team == team);

            IRenderable[,] unitRosterGrid = new IRenderable[1, teamUnits.Count];

            const int unitListHealthBarHeight = 10;
            int listPosition = 0;

            foreach (GameUnit unit in teamUnits)
            {
                IRenderable[,] unitContent =
                {
                    {
                        new RenderText(GameDriver.MapFont, unit.Id)
                    },
                    {
                        unit.MediumPortrait
                    },
                    {
                        unit.GetInitiativeHealthBar(new Vector2(unit.MediumPortrait.Width, unitListHealthBarHeight))
                    }
                };

                IRenderable singleUnitContent = new Window(
                    "Unit " + unit.Id + " Window",
                    windowTexture,
                    new WindowContentGrid(unitContent, 2),
                    TeamUtility.DetermineTeamColor(unit.Team)
                );

                unitRosterGrid[0, listPosition] = singleUnitContent;
                listPosition++;
            }

            return unitRosterGrid;
        }

        #endregion Generation

        #region Positioning

        //Blue Windows
        private Vector2 BlueTeamLeaderPortraitPosition()
        {
            //Top-Left of screen
            return new Vector2(WindowEdgeBuffer);
        }

        private Vector2 BlueTeamResultWindowPosition()
        {
            //Right of Blue Portrait, Aligned at top
            return new Vector2(
                BlueTeamLeaderPortraitPosition().X + BlueTeamLeaderPortraitWindow.Width + WindowPadding,
                BlueTeamLeaderPortraitPosition().Y
            );
        }

        private Vector2 BlueTeamUnitRosterPosition()
        {
            //Below Blue Result, Aligned at left with Result
            return new Vector2(
                BlueTeamResultWindowPosition().X,
                BlueTeamResultWindowPosition().Y + BlueTeamResultWindow.Height + WindowPadding
            );
        }


        //Versus Window
        private Vector2 VersusWindowPosition()
        {
            //Center of screen
            return new Vector2(
                GameDriver.ScreenSize.X - (float) VersusWindow.Width / 2,
                GameDriver.ScreenSize.Y - (float) VersusWindow.Height / 2
            );
            
        }


        //Red Windows
        private Vector2 RedTeamLeaderPortraitPosition()
        {
            //Bottom-Right of screen
            return new Vector2(
                GameDriver.ScreenSize.X - WindowEdgeBuffer - RedTeamLeaderPortraitWindow.Width,
                GameDriver.ScreenSize.Y - WindowEdgeBuffer - RedTeamLeaderPortraitWindow.Height
            );
        }

        private Vector2 RedTeamResultWindowPosition()
        {
            float redPortraitLeft = RedTeamLeaderPortraitPosition().X;
            float redPortraitBottom = RedTeamLeaderPortraitPosition().Y + RedTeamResultWindow.Height;

            //Left of Red Portrait, Aligned at bottom
            return new Vector2(
                redPortraitLeft - WindowPadding - RedTeamResultWindow.Width,
                redPortraitBottom
            );
        }

        private Vector2 RedTeamUnitRosterPosition()
        {
            float redResultRight = RedTeamResultWindowPosition().X + RedTeamResultWindow.Width;
            float redResultTop = RedTeamResultWindowPosition().Y;

            //Above Red Result, Aligned at right with Result
            return new Vector2(
                redResultRight - RedTeamUnitRosterWindow.Width,
                redResultTop - WindowPadding - RedTeamUnitRosterWindow.Height
            );
        }

        #endregion Positioning

        public void ToggleVisible()
        {
            Visible = !Visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}