using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Containers
{
    public interface IScene
    {
        void ToggleVisible();
        void Draw(SpriteBatch spriteBatch);
    }
}