using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class DeviceSelectOption : MenuOption
    {
        private readonly string labelText;
        private readonly ControlConfigContext.Device device;

        public DeviceSelectOption(string labelText, ControlConfigContext.Device device, Color color) :
            base(new RenderText(AssetManager.WindowFont, labelText), color, HorizontalAlignment.Centered)
        {
            this.labelText = labelText;
            this.device = device;
        }

        public override void Execute()
        {
            GlobalContext.ControlConfigContext.OpenRemapMenu(device);
        }

        public override IRenderable Clone()
        {
            return new DeviceSelectOption(labelText, device, DefaultColor);
        }
    }
}