
using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public interface IEffectTile
    {
        void TriggerStartOfTurn();
        void TriggerEndOfTurn();
        bool IsExpired { get; }
        Vector2 MapCoordinates { get; }
    }
}