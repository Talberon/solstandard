using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.Containers.UI
{
    public class SelectMapUI : IUserInterface
    {
        private Window instructionWindow;
        private Window mapInfoWindow;

        private const int WindowEdgeBuffer = 5;
        private static readonly Color InstructionWindowColor = new Color(50, 50, 50, 200);
        private static readonly Color MapInfoWindowColor = new Color(50, 50, 50, 200);

        private bool visible;

        public SelectMapUI()
        {
            SetUpWindows();
        }

        private void SetUpWindows()
        {
            WindowContentGrid instructionContentGrid = new WindowContentGrid(
                new IRenderable[,]
                {
                    {
                        new RenderText(AssetManager.WindowFont,
                            "Select a map! Move the cursor to the crossed swords and press "),
                        ButtonIconProvider.GetButton(ButtonIconProvider.ButtonIcon.A, new Vector2(25))
                    }
                },
                1
            );


            instructionWindow = new Window(
                "Instruction Window", AssetManager.WindowTexture,
                instructionContentGrid,
                InstructionWindowColor
            );

            mapInfoWindow = new Window(
                "SelectMapEntity Info Window", AssetManager.WindowTexture, new RenderBlank(), MapInfoWindowColor
            );
        }

        public void UpdateMapInfoWindow(IRenderable terrainInfo)
        {
            if (terrainInfo == null)
            {
                mapInfoWindow = null;
            }
            else
            {
                mapInfoWindow = new Window("MapInfo Window", AssetManager.WindowTexture, terrainInfo, MapInfoWindowColor);
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
        }
    }
}