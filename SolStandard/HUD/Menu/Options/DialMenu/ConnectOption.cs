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
        private NetworkMenuView NetworkMenuView { get; }
        private const string Connect = "Connect";

        public ConnectOption(Color color, NetworkMenuView networkMenuView) : base(
            new RenderText(AssetManager.WindowFont, Connect), color, HorizontalAlignment.Centered
        )
        {
            NetworkMenuView = networkMenuView;
        }

        public override void Execute()
        {
            NetworkMenuView.AttemptConnection();
        }

        public override IRenderable Clone()
        {
            return new ConnectOption(DefaultColor, NetworkMenuView);
        }
    }
}