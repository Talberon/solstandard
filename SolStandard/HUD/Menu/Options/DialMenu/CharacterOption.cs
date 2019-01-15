using Microsoft.Xna.Framework;
using SolStandard.Containers.View;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class CharacterOption : MenuOption
    {
        private readonly char character;
        private NetworkMenuView NetworkMenuView { get; set; }

        public CharacterOption(char character, Color color, NetworkMenuView networkMenuView) : base(
            new RenderText(AssetManager.MainMenuFont, character.ToString()), color
        )
        {
            this.character = character;
            NetworkMenuView = networkMenuView;
        }

        public override void Execute()
        {
            NetworkMenuView.EnterCharacter(character);
        }

        public override IRenderable Clone()
        {
            return new CharacterOption(character, DefaultColor, NetworkMenuView);
        }
    }
}