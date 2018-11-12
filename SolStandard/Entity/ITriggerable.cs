using Microsoft.Xna.Framework;

namespace SolStandard.Entity
{
    public interface ITriggerable
    {
        void Trigger();
        Vector2 MapCoordinates { get; }
    }
}