using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Network;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class BackspaceOption : MenuOption
    {
        private NetworkHUD NetworkHUD { get; }
        private const string Backspace = "<<";

        public BackspaceOption(Color color, NetworkHUD networkHUD) : base(
            new RenderText(AssetManager.MainMenuFont, Backspace), color, HorizontalAlignment.Centered
        )
        {
            NetworkHUD = networkHUD;
        }

        public override void Execute()
        {
            NetworkHUD.BackspaceCharacter();
        }

        public override IRenderable Clone()
        {
            return new BackspaceOption(DefaultColor, NetworkHUD);
        }
    }
}