using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    /*
     * MapUI is where the HUD elements for the Map Scene are handled.
     * HUD Elements in this case includes various map-screen windows.
     */
    public class MapUI : IUserInterface
    {
        private readonly Vector2 screenSize;
        private const int WindowEdgeBuffer = 5;

        public Window LeftUnitPortraitWindow { get; set; }
        public Window LeftUnitDetailWindow { get; set; }

        public Window RightUnitPortraitWindow { get; set; }
        public Window RightUnitDetailWindow { get; set; }

        public Window TurnWindow { get; set; }
        public Window InitiativeWindow { get; set; }
        public Window TerrainEntityWindow { get; set; }
        public Window HelpTextWindow { get; set; }
        
        public Window UserPromptWindow { get; private set; }

        private bool visible;

        private readonly ITexture2D windowTexture;
        
        public MapUI(Vector2 screenSize, ITexture2D windowTexture)
        {
            this.screenSize = screenSize;
            this.windowTexture = windowTexture;
            visible = true;
        }
        
        internal void GenerateUserPromptWindow(WindowContentGrid promptTextContent, Vector2 sizeOverride)
        {
            Color promptWindowColor = new Color(40, 30, 40, 200);
            UserPromptWindow = new Window("User Prompt Window", windowTexture, promptTextContent, promptWindowColor,
                sizeOverride);
        }

        private Vector2 LeftUnitPortraitWindowPosition(int portraitWindowHeight, int initiativeWindowHeight)
        {
            //Bottom-left, above initiative window
            return new Vector2(WindowEdgeBuffer, screenSize.Y - portraitWindowHeight - initiativeWindowHeight);
        }

        private Vector2 LeftUnitDetailWindowPosition(int detailWindowHeight, int leftPortraitWindowWidth,
            int initiativeWindowHeight)
        {
            //Bottom-left, right of portrait, above initiative window
            return new Vector2(WindowEdgeBuffer + leftPortraitWindowWidth,
                screenSize.Y - detailWindowHeight - initiativeWindowHeight);
        }

        private Vector2 RightUnitPortraitWindowPosition(int detailWindowHeight, int portraitWindowWidth,
            int initiativeWindowHeight)
        {
            //Bottom-right, above intiative window
            return new Vector2(screenSize.X - portraitWindowWidth - WindowEdgeBuffer,
                screenSize.Y - detailWindowHeight - initiativeWindowHeight);
        }

        private Vector2 RightUnitDetailWindowPosition(int detailWindowHeight, int detailWindowWidth,
            int rightPortraitWidth, int initiativeWindowHeight)
        {
            //Bottom-right, left of portrait, above intiative window
            return new Vector2(screenSize.X - detailWindowWidth - rightPortraitWidth - WindowEdgeBuffer,
                screenSize.Y - detailWindowHeight - initiativeWindowHeight);
        }

        private Vector2 InitiativeWindowPosition(int windowWidth, int windowHeight)
        {
            //Bottom-right
            return new Vector2(screenSize.X - windowWidth - WindowEdgeBuffer,
                screenSize.Y - windowHeight);
        }

        private Vector2 TurnWindowPosition(int windowHeight)
        {
            //Bottom-right
            return new Vector2(WindowEdgeBuffer, screenSize.Y - windowHeight);
        }

        private Vector2 TerrainWindowPosition(int windowWidth)
        {
            //Top-right
            return new Vector2(screenSize.X - windowWidth - WindowEdgeBuffer, WindowEdgeBuffer);
        }

        private Vector2 HelpTextWindowPosition()
        {
            //Top-left
            return new Vector2(WindowEdgeBuffer);
        }

        private Vector2 UserPromptWindowPosition()
        {
            //Middle of the screen
            return new Vector2(GameDriver.ScreenSize.X / 2 - (float) UserPromptWindow.Width / 2,
                GameDriver.ScreenSize.Y / 2 - (float) UserPromptWindow.Height / 2);
        }
        
        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            if (HelpTextWindow != null)
            {
                HelpTextWindow.Draw(spriteBatch, HelpTextWindowPosition());
            }

            if (TerrainEntityWindow != null)
            {
                TerrainEntityWindow.Draw(spriteBatch, TerrainWindowPosition(TerrainEntityWindow.Width));
            }

            if (TurnWindow != null)
            {
                TurnWindow.Draw(spriteBatch, TurnWindowPosition(TurnWindow.Height));
            }

            if (InitiativeWindow != null)
            {
                InitiativeWindow.Draw(spriteBatch,
                    InitiativeWindowPosition(InitiativeWindow.Width, InitiativeWindow.Height));

                if (LeftUnitPortraitWindow != null)
                {
                    LeftUnitPortraitWindow.Draw(spriteBatch,
                        LeftUnitPortraitWindowPosition(LeftUnitPortraitWindow.Height,
                            InitiativeWindow.Height));

                    if (LeftUnitDetailWindow != null)
                    {
                        LeftUnitDetailWindow.Draw(spriteBatch,
                            LeftUnitDetailWindowPosition(LeftUnitDetailWindow.Height,
                                LeftUnitPortraitWindow.Width, InitiativeWindow.Height));
                    }
                }

                if (RightUnitPortraitWindow != null)
                {
                    RightUnitPortraitWindow.Draw(spriteBatch,
                        RightUnitPortraitWindowPosition(RightUnitPortraitWindow.Height,
                            RightUnitPortraitWindow.Width, InitiativeWindow.Height));

                    if (RightUnitDetailWindow != null)
                    {
                        RightUnitDetailWindow.Draw(spriteBatch,
                            RightUnitDetailWindowPosition(RightUnitDetailWindow.Height,
                                RightUnitDetailWindow.Width, RightUnitPortraitWindow.Width,
                                InitiativeWindow.Height));
                    }
                }

                if (UserPromptWindow != null)
                {
                    UserPromptWindow.Draw(spriteBatch, UserPromptWindowPosition());
                }
            }
        }

    }
}