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
        private static string OptionText =>
            $"Disable Creeps in Multiplayer: {!GlobalContext.CreepPreferences.CreepsCanSpawn}";

        public CreepDisableOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, OptionText), color)
        {
        }

        public override void Execute()
        {
            GlobalContext.CreepPreferences.ToggleCreepsCanSpawn();
            UpdateLabel(new RenderText(AssetManager.MainMenuFont, OptionText));
        }

        public override IRenderable Clone()
        {
            return new OpenCodexOption(DefaultColor);
        }
    }
}