using System;
using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.PauseMenu
{
    public class ConcedeOption : MenuOption
    {
        public ConcedeOption(Color color) : base(new RenderText(AssetManager.MainMenuFont, "Surrender"), color)
        {
        }

        public override void Execute()
        {
            //TODO Implement win condition on surrender
            throw new NotImplementedException();
        }
    }
}