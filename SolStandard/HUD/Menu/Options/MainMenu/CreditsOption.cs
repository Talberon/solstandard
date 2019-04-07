using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
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
            GameContext.CreditsContext.OpenView();
        }

        public override IRenderable Clone()
        {
            return new CreditsOption(DefaultColor);
        }
    }
}