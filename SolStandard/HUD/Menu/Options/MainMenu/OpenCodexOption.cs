using Microsoft.Xna.Framework;
using SolStandard.Containers.Contexts;
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
            GameContext.CodexContext.OpenMenu();
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}