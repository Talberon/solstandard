using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.View
{
    public class MapSelectScreenView : IUserInterface
    {
        private Window instructionWindow;
        private Window mapInfoWindow;
        private Window teamSelectWindow;

        private const int WindowEdgeBuffer = 5;
        private static readonly Color InstructionWindowColor = new Color(50, 50, 50, 200);
        private static readonly Color MapInfoWindowColor = new Color(50, 50, 50, 200);
        private static readonly Color SelectedTeamColor = new Color(150, 135, 13, 200);

        private bool visible;

        public MapSelectScreenView()
        {
            SetUpWindows();
        }

        private void SetUpWindows()
        {
            WindowContentGrid instructionContentGrid = new WindowContentGrid(
                new [,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,
                            "Select a map! Move the cursor to the crossed swords and press "),
                        InputIconProvider.GetInputIcon(Input.Confirm,
                            new Vector2(AssetManager.WindowFont.MeasureString("A").Y))
                    }
                },
                1
            );


            instructionWindow = new Window(instructionContentGrid, InstructionWindowColor);

            mapInfoWindow = new Window(new RenderBlank(), MapInfoWindowColor);
        }

        public void UpdateTeamSelectWindow()
        {
            const int iconSize = 48;
            Color solWindowColor = (GameContext.P1Team == Team.Red) ? SelectedTeamColor : MapInfoWindowColor;
            Color lunaWindowColor = (GameContext.P1Team == Team.Blue) ? SelectedTeamColor : MapInfoWindowColor;

            WindowContentGrid teamSelectContent = new WindowContentGrid(new [,]
                {
                    {
                        InputIconProvider.GetInputIcon(Input.LeftBumper, new Vector2(iconSize)),
                        //SOL TEAM
                        new Window(new WindowContentGrid(new IRenderable[,]
                            {
                                {
                                    new Window(
                                        new RenderText(AssetManager.WindowFont,
                                            (GameContext.P1Team == Team.Red) ? "P1" : "P2"),
                                        TeamUtility.DetermineTeamColor(Team.Red)
                                    ),
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Red, new Vector2(iconSize)),
                                        Color.Transparent
                                    ),
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Red Team"),
                                        TeamUtility.DetermineTeamColor(Team.Red)
                                    ),
                                },
                            }, 1, HorizontalAlignment.Centered), solWindowColor
                        ),
                        //LUNA TEAM
                        new Window(new WindowContentGrid(new IRenderable[,]
                            {
                                {
                                    new Window(
                                        new RenderText(AssetManager.WindowFont,
                                            (GameContext.P1Team == Team.Blue) ? "P1" : "P2"),
                                        TeamUtility.DetermineTeamColor(Team.Blue)
                                    ),
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Blue, new Vector2(iconSize)),
                                        Color.Transparent
                                    ),
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Blue Team"),
                                        TeamUtility.DetermineTeamColor(Team.Blue)
                                    ),
                                },
                            }, 1, HorizontalAlignment.Centered), lunaWindowColor
                        ),
                        InputIconProvider.GetInputIcon(Input.RightBumper, new Vector2(iconSize)),
                    }
                }, 1, HorizontalAlignment.Centered
            );

            teamSelectWindow = new Window(teamSelectContent, Color.Transparent, HorizontalAlignment.Centered);
        }

        public void UpdateMapInfoWindow(IRenderable terrainInfo)
        {
            if (terrainInfo == null)
            {
                mapInfoWindow = null;
            }
            else
            {
                mapInfoWindow = new Window(terrainInfo, MapInfoWindowColor, HorizontalAlignment.Right);
            }
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Top-Left Corner
            if (instructionWindow != null)
            {
                instructionWindow.Draw(spriteBatch, new Vector2(WindowEdgeBuffer));
            }

            //Bottom-Right Corner
            if (mapInfoWindow != null)
            {
                mapInfoWindow.Draw(spriteBatch,
                    new Vector2(WindowEdgeBuffer, GameDriver.ScreenSize.Y - WindowEdgeBuffer) -
                    new Vector2(0, mapInfoWindow.Height));
            }

            if (teamSelectWindow != null)
            {
                teamSelectWindow.Draw(spriteBatch,
                    new Vector2(
                        (GameDriver.ScreenSize.X / 2) - ((float) teamSelectWindow.Width / 2),
                        GameDriver.ScreenSize.Y - WindowEdgeBuffer - teamSelectWindow.Height
                    )
                );
            }
        }
    }
}