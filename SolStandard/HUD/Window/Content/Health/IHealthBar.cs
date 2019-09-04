using Microsoft.Xna.Framework;
using SolStandard.Utility;

namespace SolStandard.HUD.Window.Content.Health
{
    public interface IHealthBar : IRenderable
    {
        Vector2 BarSize { set; }
        void SetArmorAndHp(int armor, int hp);
    }
}