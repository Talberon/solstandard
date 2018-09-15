using Microsoft.Xna.Framework.Graphics;
using SolStandard.Utility.Monogame;

namespace SolStandard.Containers.UI
{
    public class MapSelectionMenuUI : IUserInterface
    {
        //TODO Implement me
        //Show grid of cells that have miniature previews of maps
        //Have cursor for active player related to an option in the map list
        //Show big preview of map when hovered
        //Show Game type and previews of each Team Leader next to the map preview
        private readonly ITexture2D windowTexture;

        public MapSelectionMenuUI(ITexture2D windowTexture)
        {
            this.windowTexture = windowTexture;
        }

        public void ToggleVisible()
        {
            throw new System.NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new System.NotImplementedException();
        }
    }
}