using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.MainMenu
{
    public class OpenCodexOption : MenuOption
    {
        private const string CodexOptionText = "View Codex";

        public OpenCodexOption(Color windowColor) :
            base(new RenderText(AssetManager.MainMenuFont, CodexOptionText), windowColor)
        {
        }

        public override void Execute()
        {
            GlobalContext.CodexContext.OpenMenu();
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}