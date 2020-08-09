using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Network;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class ConnectOption : MenuOption
    {
        private NetworkHUD NetworkHUD { get; }
        private const string Connect = "Connect";

        public ConnectOption(Color color, NetworkHUD networkHUD) : base(
            new RenderText(AssetManager.WindowFont, Connect), color, HorizontalAlignment.Centered
        )
        {
            NetworkHUD = networkHUD;
        }

        public override void Execute()
        {
            NetworkHUD.AttemptConnection();
        }

        public override IRenderable Clone()
        {
            return new ConnectOption(DefaultColor, NetworkHUD);
        }
    }
}