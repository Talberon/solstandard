
using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public interface IEffectTile
    {
        void TriggerEffect();
        bool IsExpired { get; }
        Vector2 MapCoordinates { get; }
    }
}