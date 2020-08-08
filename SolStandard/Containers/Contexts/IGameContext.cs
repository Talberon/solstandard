using Microsoft.Xna.Framework;
using Steelbreakers.Contexts.Components.Views;
using Steelbreakers.Utility.GUI.HUD.Menus;

namespace SolStandard.Containers.Contexts
{
    public interface IGameContext
    {
        IHUDView View { get; }
        MenuContainer MenuContainer { get; }
        void Update(GameTime gameTime);
    }
}