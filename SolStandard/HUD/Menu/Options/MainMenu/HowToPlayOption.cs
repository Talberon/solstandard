using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class HowToPlayOption : MenuOption
    {
        public HowToPlayOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, "How To Play"), windowColor)
        {
        }

        public override void Execute()
        {
            GlobalContext.HowToPlayContext.OpenView();
        }

        public override IRenderable Clone()
        {
            return new HowToPlayOption(DefaultColor);
        }
    }
}