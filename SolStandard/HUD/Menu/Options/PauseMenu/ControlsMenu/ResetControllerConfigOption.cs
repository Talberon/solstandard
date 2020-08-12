using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.Containers.Components.InputRemapping;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class ResetControllerConfigOption : MenuOption
    {
        private readonly string label;
        private readonly ControlConfigContext.Device deviceType;

        public ResetControllerConfigOption(string label, ControlConfigContext.Device deviceType, Color color) :
            base(new RenderText(AssetManager.WindowFont, label), color, HorizontalAlignment.Centered)
        {
            this.label = label;
            this.deviceType = deviceType;
        }

        public override void Execute()
        {
            GlobalContext.ControlConfigContext.ResetMetaController(deviceType);
        }

        public override IRenderable Clone()
        {
            return new ResetControllerConfigOption(label, deviceType, DefaultColor);
        }
    }
}