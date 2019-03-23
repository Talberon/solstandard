using Microsoft.Xna.Framework;
using SolStandard.HUD.Window;
using SolStandard.Utility;
using SolStandard.Utility.Assets;

namespace SolStandard.HUD.Menu.Options
{
    public class UnselectableOption : MenuOption
    {
        public UnselectableOption(IRenderable labelContent, Color color,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left) :
            base(labelContent, color, horizontalAlignment)
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