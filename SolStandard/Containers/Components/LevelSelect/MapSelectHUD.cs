using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.Containers.Components.Global;
using SolStandard.Entity.Unit;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Animation;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Inputs;
using HorizontalAlignment = SolStandard.HUD.Window.HorizontalAlignment;


namespace SolStandard.Containers.Components.LevelSelect
{
    public class MapSelectHUD : IUserInterface
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

        public MapSelectHUD()
        {
            SetUpWindows();
        }

        private static IRenderableAnimation LeftSideWindowAnimation =>
            new RenderableSlide(RenderableSlide.SlideDirection.Right, WindowSlideDistance, WindowSlideSpeed);

        private void SetUpWindows()
        {
            int iconSize = Convert.ToInt32(AssetManager.WindowFont.MeasureString("A").Y);

            var instructionContentGrid = new WindowContentGrid(
                new[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,
                            "Select a map! Move the cursor to the crossed swords and press "),
                        InputIconProvider.GetInputIcon(Input.Confirm, iconSize),
                        RenderBlank.Blank,
                        RenderBlank.Blank,
                    },
                    {
                        new RenderText(AssetManager.WindowFont, "Toggle between maps with"),
                        InputIconProvider.GetInputIcon(Input.TabLeft, iconSize),
                        new RenderText(AssetManager.WindowFont, "and"),
                        InputIconProvider.GetInputIcon(Input.TabRight, iconSize),
                    }
                }
            );


            instructionWindow = new Window(instructionContentGrid, InstructionWindowColor);

            mapInfoWindow =
                new AnimatedRenderable(new Window(RenderBlank.Blank, MapInfoWindowColor), LeftSideWindowAnimation);
        }

        public void UpdateTeamSelectWindow()
        {
            const int iconSize = 48;
            Color solWindowColor = (GlobalContext.P1Team == Team.Red) ? SelectedTeamColor : MapInfoWindowColor;
            Color lunaWindowColor = (GlobalContext.P1Team == Team.Blue) ? SelectedTeamColor : MapInfoWindowColor;

            var teamSelectContent = new WindowContentGrid(new[,]
                {
                    {
                        InputIconProvider.GetInputIcon(Input.PreviewUnit, iconSize),
                        //SOL TEAM
                        new Window(new WindowContentGrid(new IRenderable[,]
                            {
                                {
                                    new Window(
                                        new RenderText(AssetManager.WindowFont,
                                            (GlobalContext.P1Team == Team.Red) ? "P1" : "P2"),
                                        TeamUtility.DetermineTeamWindowColor(Team.Red)
                                    )
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Red, new Vector2(iconSize)),
                                        Color.Transparent
                                    )
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Red Team"),
                                        TeamUtility.DetermineTeamWindowColor(Team.Red)
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
                                            (GlobalContext.P1Team == Team.Blue) ? "P1" : "P2"),
                                        TeamUtility.DetermineTeamWindowColor(Team.Blue)
                                    )
                                },
                                {
                                    new Window(TeamIconProvider.GetTeamIcon(Team.Blue, new Vector2(iconSize)),
                                        Color.Transparent
                                    )
                                },
                                {
                                    new Window(new RenderText(AssetManager.WindowFont, "Blue Team"),
                                        TeamUtility.DetermineTeamWindowColor(Team.Blue)
                                    )
                                }
                            }, 1, HorizontalAlignment.Centered), lunaWindowColor
                        ),
                        InputIconProvider.GetInputIcon(Input.PreviewItem, iconSize)
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