using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Contexts;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Buttons;

namespace SolStandard.Containers.View
{
    public class MapSelectScreenView : IUserInterface
    {
        private Window instructionWindow;
        private AnimatedRenderable mapInfoWindow;
        private Window teamSelectWindow;

        private const int WindowSlideSpeed = 40;
        private const int WindowSlideDistance = 300;

        private const int WindowEdgeBuffer = 5;
        private static readonly Color InstructionWindowColor = new Color(50, 50, 50, 200);
        private static readonly Color MapInfoWindowColor = new Color(50, 50, 50, 200);
        private static readonly Color SelectedTeamColor = new Color(150, 135, 13, 200);

        private bool visible;

        public MapSelectScreenView()
        {
            SetUpWindows();
        }

        private static IRenderableAnimation LeftSideWindowAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Right, WindowSlideDistance, WindowSlideSpeed);

        private void SetUpWindows()
        {
            Vector2 iconSize = new Vector2(AssetManager.WindowFont.MeasureString("A").Y);

            WindowContentGrid instructionContentGrid = new WindowContentGrid(
                new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,
                            "Select a map! Move the cursor to the crossed swords and press "),
                        InputIconProvider.GetInputIcon(Input.Confirm, iconSize),
                        new RenderBlank(),
                        new RenderBlank(),
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Toggle between maps with"),
                        InputIconProvider.GetInputIcon(Input.TabLeft, iconSize),
                        new RenderText(AssetManager.WindowFont, "and"),
                        InputIconProvider.GetInputIcon(Input.TabRight, iconSize),
                    }
                },
                1
            );


            instructionWindow = new Window(instructionContentGrid, InstructionWindowColor);

            mapInfoWindow =
                new AnimatedRenderable(new Window(new RenderBlank(), MapInfoWindowColor), LeftSideWindowAnimation);
        }

        public void UpdateTeamSelectWindow()
        {
            const int iconSize = 48;
            Color solWindowColor = (GameContext.P1Team == Team.Red) ? SelectedTeamColor : MapInfoWindowColor;
            Color lunaWindowColor = (GameContext.P1Team == Team.Blue) ? SelectedTeamColor : MapInfoWindowColor;

            WindowContentGrid teamSelectContent = new WindowContentGrid(new[,]
                {
                    {
                        InputIconProvider.GetInputIcon(Input.PreviewUnit, new Vector2(iconSize)),
                        //SOL TEAM
                        new Window(new WindowContentGrid(new IRenderable[,]
                            {
                                {
                                    new Window(
                                        new RenderText(AssetManager.WindowFont,
                                            (GameContext.P1Team == Team.Red) ? "P1" : "P2"),
                                        TeamUtility.DetermineTeamColor(Team.Red)
                                    )
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Red, new Vector2(iconSize)),
                                        Color.Transparent
                                    )
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Red Team"),
                                        TeamUtility.DetermineTeamColor(Team.Red)
                                    )
                                }
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
                                    )
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Blue, new Vector2(iconSize)),
                                        Color.Transparent
                                    )
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Blue Team"),
                                        TeamUtility.DetermineTeamColor(Team.Blue)
                                    )
                                }
                            }, 1, HorizontalAlignment.Centered), lunaWindowColor
                        ),
                        InputIconProvider.GetInputIcon(Input.PreviewItem, new Vector2(iconSize))
                    }
                }, 1, HorizontalAlignment.Centered
            );

            teamSelectWindow = new Window(teamSelectContent, Color.Transparent, HorizontalAlignment.Centered);
        }

        public void UpdateMapInfoWindow(IRenderable terrainInfo)
        {
            mapInfoWindow = terrainInfo == null
                ? null
                : new AnimatedRenderable(new Window(terrainInfo, MapInfoWindowColor, HorizontalAlignment.Right),
                    LeftSideWindowAnimation);
        }

        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Top-Left Corner
            instructionWindow?.Draw(spriteBatch, new Vector2(WindowEdgeBuffer));

            //Bottom-Right Corner
            mapInfoWindow?.Draw(spriteBatch,
                new Vector2(WindowEdgeBuffer, GameDriver.ScreenSize.Y - WindowEdgeBuffer) -
                new Vector2(0, mapInfoWindow.Height));

            teamSelectWindow?.Draw(spriteBatch,
                new Vector2(
                    (GameDriver.ScreenSize.X / 2) - ((float) teamSelectWindow.Width / 2),
                    GameDriver.ScreenSize.Y - WindowEdgeBuffer - teamSelectWindow.Height
                )
            );
        }
    }
}