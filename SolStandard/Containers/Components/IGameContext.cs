using Microsoft.Xna.Framework;
using SolStandard.NeoGFX.GUI;
using SolStandard.NeoGFX.GUI.Menus;

namespace SolStandard.Containers.Components
{
    public interface IGameContext
    {
        IHUDView View { get; }
        MenuContainer MenuContainer { get; }
        void Update(GameTime gameTime);
    }
}