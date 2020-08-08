using Microsoft.Xna.Framework;

namespace SolStandard.NeoGFX.Graphics
{
    public interface IPositionedRenderable : IRenderable
    {
        Vector2 TopLeftPoint { get; set; }

        Vector2 CenterPoint
        {
            get => TopLeftPoint + new Vector2(Width, Height) / 2;
            set => TopLeftPoint = value - new Vector2(Width, Height) / 2;
        }
    }
}