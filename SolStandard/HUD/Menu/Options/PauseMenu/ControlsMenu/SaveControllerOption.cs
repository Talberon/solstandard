using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ControlsMenu
{
    public class SaveControllerOption : MenuOption
    {
        private readonly string label;

        public SaveControllerOption(string label, Color color) :
            base(new RenderText(AssetManager.WindowFont, label), color, HorizontalAlignment.Centered)
        {
            this.label = label;
        }

        public override void Execute()
        {
            GlobalContext.ControlConfigContext.SaveControlMappings();
        }

        public override IRenderable Clone()
        {
            return new SaveControllerOption(label, DefaultColor);
        }
    }
}