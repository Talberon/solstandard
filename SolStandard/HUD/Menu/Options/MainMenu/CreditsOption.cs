using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class CreditsOption : MenuOption
    {
        public CreditsOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, "Credits"), windowColor)
        {
        }

        public override void Execute()
        {
            GlobalContext.CreditsContext.OpenView();
        }

        public override IRenderable Clone()
        {
            return new CreditsOption(DefaultColor);
        }
    }
}