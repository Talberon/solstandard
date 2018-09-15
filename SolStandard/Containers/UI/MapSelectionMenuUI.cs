using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolStandard.HUD.Menu;
using SolStandard.HUD.Menu.Options;
using SolStandard.HUD.Menu.Options.MapSelectMenu;
using SolStandard.Utility;

namespace SolStandard.Containers.UI
{
    public class MapSelectionMenuUI : IUserInterface
    {
        //TODO Implement me
        //Show grid of cells that have miniature previews of maps
        //Have cursor for active player related to an option in the map list
        //Show big preview of map when hovered
        //Show Game type and previews of each Team Leader next to the map preview
        private readonly VerticalMenu mapSelectMenu;
        private bool visible;

        public MapSelectionMenuUI()
        {
            mapSelectMenu = GenerateMapSelectionMenu();
            visible = true;
        }

        public VerticalMenu MapSelectMenu
        {
            get { return mapSelectMenu; }
        }

        private static VerticalMenu GenerateMapSelectionMenu()
        {
            IOption[] options = new IOption[GameDriver.MapFiles.Count];

            for (int i = 0; i < options.Length; i++)
            {
                options[i] = new MapSelectOption(GameDriver.MapFiles.ElementAt(i).Key,
                    GameDriver.MapFiles.ElementAt(i).Value);
            }

            IRenderable cursorSprite = new SpriteAtlas(GameDriver.MenuCursorTexture,
                new Vector2(GameDriver.MenuCursorTexture.Width, GameDriver.MenuCursorTexture.Height), 1);

            return new VerticalMenu(options, cursorSprite);
        }


        public void ToggleVisible()
        {
            visible = !visible;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                Vector2 centerScreen = GameDriver.ScreenSize / 2;
                Vector2 mapSelectMenuCenter = new Vector2(mapSelectMenu.Width, mapSelectMenu.Height) / 2;

                Vector2 mapSelectMenuPosition =
                    new Vector2(centerScreen.X - mapSelectMenuCenter.X, centerScreen.Y - mapSelectMenuCenter.Y);

                mapSelectMenu.Draw(spriteBatch, mapSelectMenuPosition);
            }
        }
    }
}