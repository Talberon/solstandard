using Microsoft.Xna.Framework.Graphics;

namespace SolStandard.Containers
{
    public interface IUserInterface
    {
        void ToggleVisible();
        void Draw(SpriteBatch spriteBatch);
    }
}