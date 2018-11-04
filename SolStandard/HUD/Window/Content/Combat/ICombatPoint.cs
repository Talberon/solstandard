using Microsoft.Xna.Framework;

namespace SolStandard.HUD.Window.Content.Combat
{
    public interface ICombatPoint
    {
        bool Enabled { get; }
        void Disable(Color disabledColor);
    }
}