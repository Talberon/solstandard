using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Network;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class SequelPromoOption : MenuOption
    {
        public SequelPromoOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, "Super Sol Standard", Color.Yellow), windowColor)
        {
        }

        public override void Execute()
        {
            AssetManager.MenuConfirmSFX.Play();

            Browser.OpenUrl("https://store.steampowered.com/app/1756730/Super_Sol_Standard/");
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}