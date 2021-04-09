using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class MusicMuteOption : MenuOption
    {
        private const string OnLabel = "Music: On";
        private const string OffLabel = "Music: Off";

        public MusicMuteOption(Color color) : base(
            new RenderText(AssetManager.WindowFont, (MusicBox.Muted) ? OffLabel : OnLabel),
            color
        )
        {
        }

        public override void Execute()
        {
            MusicBox.ToggleMute();
            UpdateLabel(new RenderText(AssetManager.WindowFont, (MusicBox.Muted) ? OffLabel : OnLabel));
        }

        public override IRenderable Clone()
        {
            return new MusicMuteOption(DefaultColor);
        }
    }
}