using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Global;
using SolStandard.HUD.Menu.Options.MainMenu;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class CreepDisableOption : MenuOption
    {
        public static string OptionText =>
            $"Disable Creeps in 2P: <{(!CreepPreferences.Instance.CreepsCanSpawn).ToString().ToUpper()}>";

        public CreepDisableOption(Color color) : base(new RenderText(AssetManager.WindowFont, OptionText), color)
        {
        }

        public override void Execute()
        {
            CreepPreferences.Instance.ToggleCreepsCanSpawn();
            UpdateLabel(new RenderText(AssetManager.WindowFont, OptionText));
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}