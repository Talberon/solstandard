using Microsoft.Xna.Framework;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options
{
    public class UnselectableOption : MenuOption
    {
        public UnselectableOption(IRenderable labelContent, Color color) : base(labelContent, color)
        {
        }

        public override void Execute()
        {
            AssetManager.WarningSFX.Play();
        }

        public override IRenderable Clone()
        {
            return new UnselectableOption(LabelContent, DefaultColor);
        }
    }
}