using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public interface IResourcePoint : IRenderable
    {
        bool Active { set; }
        Vector2 Size { set; }
    }
}