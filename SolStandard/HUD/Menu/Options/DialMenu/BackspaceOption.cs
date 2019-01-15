using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class BackspaceOption : MenuOption
    {
        private NetworkMenuView NetworkMenuView { get; set; }
        private const string Backspace = "<<";

        public BackspaceOption(Color color, NetworkMenuView networkMenuView) : base(
            new RenderText(AssetManager.MainMenuFont, Backspace), color
        )
        {
            NetworkMenuView = networkMenuView;
        }

        public override void Execute()
        {
            NetworkMenuView.BackspaceCharacter();
        }

        public override IRenderable Clone()
        {
            return new BackspaceOption(DefaultColor, NetworkMenuView);
        }
    }
}