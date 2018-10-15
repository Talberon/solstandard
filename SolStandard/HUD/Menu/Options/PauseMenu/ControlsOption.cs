using System;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ControlsOption : MenuOption
    {
        public ControlsOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, "Controls"), color)
        {
        }

        public override void Execute()
        {
            //Show Control Scheme
            throw new NotImplementedException();
        }
    }
}