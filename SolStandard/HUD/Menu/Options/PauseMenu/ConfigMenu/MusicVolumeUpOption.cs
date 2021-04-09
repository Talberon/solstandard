using Microsoft.Xna.Framework;
using SolStandard.HUD.Window.Content;
using SolStandard.Utility;
using SolStandard.Utility.Assets;
using SolStandard.Utility.Monogame;

namespace SolStandard.HUD.Menu.Options.PauseMenu.ConfigMenu
{
    public class MusicVolumeUpOption : MenuOption
    {
        private const string VolumeLabel = "Music Volume Up";

        public MusicVolumeUpOption(Color color) : base(new RenderText(AssetManager.WindowFont, VolumeLabel), color)
        {
        }

        public override void Execute()
        {
            MusicBox.IncreaseVolume(0.1f);
        }

        public override IRenderable Clone()
        {
            return new MusicVolumeUpOption(DefaultColor);
        }
    }
}