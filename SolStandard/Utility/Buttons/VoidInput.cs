using SolStandard.HUD.Window.Content;

namespace SolStandard.Utility.Buttons
{
    public class VoidInput : GameControl
    {
        public override bool Pressed => false;

        public override IRenderable GetInputIcon(int iconSize)
        {
            return RenderBlank.Blank;
        }
    }
}