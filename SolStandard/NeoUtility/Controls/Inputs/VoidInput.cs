using System;

namespace SolStandard.NeoUtility.Controls.Inputs
{
    [Serializable]
    public class VoidInput : GameControl
    {
        public override bool Pressed => false;

        // public override IRenderable GetInputIcon(int iconSize)
        // {
        //     return RenderBlank.Blank;
        // }
    }
}