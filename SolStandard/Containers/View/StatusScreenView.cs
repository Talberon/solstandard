using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.View
{
    public class StatusScreenView : IUserInterface
    {
        private const int WindowEdgeBuffer = 5;
        private const int WindowPadding = 10;
        private const int MaxMediumPortraits = 9;

        private static readonly Color BackgroundColor = new Color(0, 0, 0, 100);

        private Window BlueTeamLeaderPortrait { get; set; }
        private Window BlueTeamUnitRoster { get; set; }
        private Window BlueTeamResult { get; set; }

        private Window ResultsLabelWindow { get; set; }

        private Window RedTeamLeaderPortrait { get; set; }
        private Window RedTeamUnitRoster { get; set; }
        private Window RedTeamResult { get; set; }

        public string BlueTeamResultText { private get; set; }
        public string RedTeamResultText { private get; set; }
        public IRenderable ResultLabelContent { private get; set; }

        private bool Visible { get; set; }

        public StatusScreenView()
        {
            BlueTeamResultText = "FIGHT!";
            RedTeamResultText = "FIGHT!";
            ResultLabelContent = new RenderText(AssetManager.ResultsFont, "STATUS");
        }

        public void UpdateWindows()
        {
            if (GameContext.Units.Any(unit => unit.Team == Team.Blue))
            {
                GenerateBlueTeamLeaderPortraitWindow();
                GenerateBlueTeamUnitRosterWindow();
                GenerateBlueTeamResultWindow(BlueTeamResultText);
            }

            if (GameContext.Units.Any(unit => unit.Team == Team.Red))
            {
                GenerateRedTeamLeaderPortraitWindow();
                GenerateRedTeamUnitRosterWindow();
                GenerateRedTeamResultWindow(RedTeamResultText);
            }

            GenerateResultsLabelWindow();
        }

        #region Generation

        private void GenerateResultsLabelWindow()
        {
            ResultsLabelWindow = new Window(ResultLabelContent, BackgroundColor);
        }

        private void GenerateBlueTeamLeaderPortraitWindow()
        {
            IRenderable[,] blueLeaderContent = LeaderContent(FindTeamLeader(Team.Blue));

            BlueTeamLeaderPortrait = new Window(
                new WindowContentGrid(blueLeaderContent, 1),
                TeamUtility.DetermineTeamColor(Team.Blue)
            );
        }

        private void GenerateBlueTeamUnitRosterWindow()
        {
            BlueTeamUnitRoster = new Window(
                new WindowContentGrid(GenerateUnitRoster(Team.Blue), 2),
                BackgroundColor
            );
        }

        private void GenerateBlueTeamResultWindow(string windowText)
        {
            BlueTeamResult = new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.ResultsFont, windowText)
                        }
                    },
                    1
                ),
                TeamUtility.DetermineTeamColor(Team.Blue)
            );
        }

        private void GenerateRedTeamLeaderPortraitWindow()
        {
            IRenderable[,] redLeaderContent = LeaderContent(FindTeamLeader(Team.Red));

            RedTeamLeaderPortrait = new Window(
                new WindowContentGrid(redLeaderContent, 1),
                TeamUtility.DetermineTeamColor(Team.Red)
            );
        }

        private void GenerateRedTeamUnitRosterWindow()
        {
            RedTeamUnitRoster = new Window(
                new WindowContentGrid(GenerateUnitRoster(Team.Red), 2),
                BackgroundColor
            );
        }

        private void GenerateRedTeamResultWindow(string windowText)
        {
            RedTeamResult = new Window(
                new WindowContentGrid(
                    new IRenderable[,]
                    {
                        {
                            new RenderText(AssetManager.ResultsFont, windowText)
                        }
                    },
                    1
                ),
                TeamUtility.DetermineTeamColor(Team.Red)
            );
        }

        private static GameUnit FindTeamLeader(Team team)
        {
            return GameContext.Units.Find(unit => unit.Team == team && unit.IsCommander);
        }

        private static IRenderable[,] LeaderContent(GameUnit leader)
        {
            IRenderable[,] leaderContent =
            {
                {
                    leader.LargePortrait
                }
            };
            return leaderContent;
        }


        private IRenderable[,] GenerateUnitRoster(Team team)
        {
            List<GameUnit> teamUnits = GameContext.Units.FindAll(unit => unit.Team == team);

            IRenderable[,] unitRosterGrid = new IRenderable[1, teamUnits.Count];

            const int unitListHealthBarHeight = 20;
            int listPosition = 0;


            foreach (GameUnit unit in teamUnits)
            {
                IRenderable portraitToUse =
                    (teamUnits.Count > MaxMediumPortraits) ? unit.SmallPortrait : unit.MediumPortrait;

                const int crownIconSize = 24;

                IRenderable[,] unitContent =
                {
                    {
                        unit.IsCommander
                            ? GameUnit.GetCommanderCrown(new Vector2(crownIconSize))
                            : new RenderBlank() as IRenderable,
                        new RenderText(AssetManager.WindowFont, unit.Id)
                    },
                    {
                        new RenderBlank(),
                        portraitToUse
                    },
                    {
                        new RenderBlank(),
                        unit.GetResultsHealthBar(new Vector2(portraitToUse.Width, unitListHealthBarHeight))
                    },
                    {
                        new RenderBlank(),
                        new Window(
                            new WindowContentGrid(
                                new IRenderable[,]
                                {
                                    {
                                        new SpriteAtlas(AssetManager.GoldIcon, GameDriver.CellSizeVector),
                                        new RenderText(AssetManager.WindowFont, unit.CurrentGold + "G")
                                    }
                                },
                                1
                            ),
                            TeamUtility.DetermineTeamColor(unit.Team)
                        )
                    }
                };

                IRenderable singleUnitContent = new Window(
                    new WindowContentGrid(unitContent, 2, HorizontalAlignment.Centered),
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

        private Vector2 BlueTeamResultPosition()
        {
//Right of Blue Portrait, Aligned at top
            return new Vector2(
                BlueTeamLeaderPortraitPosition().X + BlueTeamLeaderPortrait.Width + WindowPadding,
                BlueTeamLeaderPortraitPosition().Y
            );
        }

        private Vector2 BlueTeamUnitRosterPosition()
        {
//Below Blue Result, Aligned at left with Result
            return new Vector2(
                BlueTeamResultPosition().X,
                BlueTeamResultPosition().Y + BlueTeamResult.Height + WindowPadding
            );
        }

//Versus Window
        private Vector2 ResultsLabelWindowPosition()
        {
//Center of screen
            return new Vector2(
                GameDriver.ScreenSize.X / 2 - (float) ResultsLabelWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) ResultsLabelWindow.Height / 2
            );
        }

//Red Windows
        private Vector2 RedTeamLeaderPortraitPosition()
        {
//Bottom-Right of screen
            return new Vector2(
                GameDriver.ScreenSize.X - WindowEdgeBuffer - RedTeamLeaderPortrait.Width,
                GameDriver.ScreenSize.Y - WindowEdgeBuffer - RedTeamLeaderPortrait.Height
            );
        }

        private Vector2 RedTeamResultPosition()
        {
            float redPortraitLeft = RedTeamLeaderPortraitPosition().X;
            float redPortraitBottom = RedTeamLeaderPortraitPosition().Y + RedTeamLeaderPortrait.Height;

//Left of Red Portrait, Aligned at bottom
            return new Vector2(
                redPortraitLeft - WindowPadding - RedTeamResult.Width,
                redPortraitBottom - RedTeamResult.Height
            );
        }

        private Vector2 RedTeamUnitRosterPosition()
        {
            float redResultRight = RedTeamResultPosition().X + RedTeamResult.Width;
            float redResultTop = RedTeamResultPosition().Y;

//Above Red Result, Aligned at right with Result
            return new Vector2(
                redResultRight - RedTeamUnitRoster.Width,
                redResultTop - WindowPadding - RedTeamUnitRoster.Height
            );
        }

        #endregion Positioning

        public void ToggleVisible()
        {
            Visible = !Visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BlueTeamLeaderPortrait?.Draw(spriteBatch, BlueTeamLeaderPortraitPosition());
            BlueTeamUnitRoster?.Draw(spriteBatch, BlueTeamUnitRosterPosition());
            BlueTeamResult?.Draw(spriteBatch, BlueTeamResultPosition());
            ResultsLabelWindow?.Draw(spriteBatch, ResultsLabelWindowPosition());

            RedTeamLeaderPortrait?.Draw(spriteBatch, RedTeamLeaderPortraitPosition());
            RedTeamUnitRoster?.Draw(spriteBatch, RedTeamUnitRosterPosition());
            RedTeamResult?.Draw(spriteBatch, RedTeamResultPosition());
        }
    }
}