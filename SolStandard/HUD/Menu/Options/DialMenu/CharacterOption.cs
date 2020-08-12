using Microsoft.Xna.Framework;
using SolStandard.Containers.Components.Network;
using SolStandard.HUD.Window;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options.DialMenu
{
    public class CharacterOption : MenuOption
    {
        private readonly char character;
        private NetworkHUD NetworkHUD { get; }

        public CharacterOption(char character, Color color, NetworkHUD networkHUD) : base(
            new RenderText(AssetManager.MainMenuFont, character.ToString()), color, HorizontalAlignment.Centered
        )
        {
            this.character = character;
            NetworkHUD = networkHUD;
        }

        public override void Execute()
        {
            NetworkHUD.EnterCharacter(character);
        }

        public override IRenderable Clone()
        {
            return new CharacterOption(character, DefaultColor, NetworkHUD);
        }
    }
}