using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Network;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class CopyIPAddressOption : MenuOption
    {
        private readonly NetworkHUD menu;

        public CopyIPAddressOption(Color menuColor, NetworkHUD menu) : base(
            new RenderText(AssetManager.WindowFont, "Copy IP"),
            menuColor,
            HorizontalAlignment.Centered
        )
        {
            this.menu = menu;
        }

        public override void Execute()
        {
            menu.CopyHostIPAddress();
        }

        public override IRenderable Clone()
        {
            return new CopyIPAddressOption(DefaultColor, menu);
        }
    }
}