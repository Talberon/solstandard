using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class MusicVolumeDownOption : MenuOption
    {
        private const string VolumeLabel = "Music Volume Down";

        public MusicVolumeDownOption(Color color) : base(new RenderText(AssetManager.WindowFont, VolumeLabel), color)
        {
        }

        public override void Execute()
        {
            MusicBox.ReduceVolume(0.1f);
        }

        public override IRenderable Clone()
        {
            return new MusicVolumeDownOption(DefaultColor);
        }
    }
}