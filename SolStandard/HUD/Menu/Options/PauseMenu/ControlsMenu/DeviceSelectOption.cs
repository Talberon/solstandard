using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class DeviceSelectOption : MenuOption
    {
        public DeviceSelectOption(string labelText, Color color) :
            base(new RenderText(AssetManager.WindowFont, labelText), color, HorizontalAlignment.Centered)
        {
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override IRenderable Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}